﻿//    AirScout Aircraft Scatter Prediction
//    Copyright (C) DL2ALF
//
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using GMap.NET.WindowsForms.ToolTips;
using System.IO;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Zeptomoby.OrbitTools;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Configuration;
using WinTest;
using System.Diagnostics;
using AquaControls;
using AirScout.Core;
using AirScout.Aircrafts;
using NDde;
using NDde.Server;
using NDde.Client;
using ScoutBase;
using ScoutBase.Core;
using ScoutBase.Elevation;
using ScoutBase.Stations;
using ScoutBase.Propagation;
using SerializableGenerics;
using Ionic.Zip;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SQLiteDatabase;
using System.Xml;
using System.Xml.Serialization;
using System.Security.Cryptography;
using OxyPlot;
using OxyPlot.WindowsForms;
using OxyPlot.Series;
using OxyPlot.Axes;
using System.Data.SQLite;
using MimeTypes;
using AirScout.PlaneFeeds;
using AirScout.PlaneFeeds.Plugin.MEFContract;

namespace AirScout
{
    public partial class MapDlg
    {
        #region Webserver

        private void bw_Webserver_DoWork(object sender, DoWorkEventArgs e)
        {
            // name the thread for debugging
            if (String.IsNullOrEmpty(Thread.CurrentThread.Name))
                Thread.CurrentThread.Name = this.GetType().Name;

            string tmpdir = Application.StartupPath;
            // get temp directory from arguments
            if (e != null)
            {
                tmpdir = ((WebserverStartArgs)e.Argument).TmpDirectory;
            }

            // get plane feeds
            List<IPlaneFeedPlugin> planefeedplugins = new List<IPlaneFeedPlugin>();
            if (e != null)
            {
                planefeedplugins = ((WebserverStartArgs)e.Argument).PlaneFeedPlugins;
            }

            string webserverdir = Path.Combine(Application.StartupPath, "wwwroot");
            // get webserver directory from arguments
            if (e != null)
            {
                webserverdir = ((WebserverStartArgs)e.Argument).WebserverDirectory;
            }

            Log.WriteMessage("started.");
            // run simple web server
            string hosturl = "http://+:" + Properties.Settings.Default.Webserver_Port.ToString() + "/";
            string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            HttpListener listener = null;
            while (!bw_Webserver.CancellationPending)
            {
                string[] prefixes = new string[1];
                prefixes[0] = hosturl;
                int id = 0;
                try
                {
                    if (!HttpListener.IsSupported)
                    {
                        Console.WriteLine("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
                        return;
                    }
                    // URI prefixes are required,
                    if (prefixes == null || prefixes.Length == 0)
                        throw new ArgumentException("prefixes");

                    // Create a listener.
                    listener = new HttpListener();
                    // Add the prefixes.
                    foreach (string s in prefixes)
                    {
                        listener.Prefixes.Add(s);
                    }
                    listener.Start();
                    Console.WriteLine("Listening...");
                    while (!bw_Webserver.CancellationPending)
                    {
                        // Note: The GetContext method blocks while waiting for a request. 
                        HttpListenerContext context = listener.GetContext();
                        List<PlaneInfo> allplanes = Planes.GetAll(DateTime.UtcNow, Properties.Settings.Default.Planes_Position_TTL);
                        id++;
                        // send response from a background thread
                        WebServerDelivererArgs args = new WebServerDelivererArgs();
                        args.ID = id;
                        args.TmpDirectory = tmpdir;
                        args.WebserverDirectory = webserverdir;
                        args.Context = context;
                        args.AllPlanes = allplanes;
                        args.PlaneFeedPlugins = planefeedplugins;
                        WebserverDeliver bw = new WebserverDeliver();
                        bw.RunWorkerAsync(args);
                    }
                }
                catch (HttpListenerException ex)
                {
                    if (ex.ErrorCode == 5)
                    {
                        // gain additional access rights for that specific host url
                        // user will be prompted for allow changes
                        // DO NOT USE THE "listener=yes" option as recommended!!!
                        string args = "http add urlacl " + hosturl + " user=" + userName;
                        ProcessStartInfo psi = new ProcessStartInfo("netsh", args);
                        psi.Verb = "runas";
                        psi.CreateNoWindow = true;
                        psi.WindowStyle = ProcessWindowStyle.Hidden;
                        psi.UseShellExecute = true;

                        Process.Start(psi).WaitForExit();
                    }
                    // do almost nothing
                    // wait 10 seconds and restart the listener
                    Thread.Sleep(10000);
                }
                catch (Exception ex)
                {
                    // do almost nothing
                    // wait 10 seconds and restart the listener
                    Thread.Sleep(10000);
                }
                finally
                {
                }
            }
            if (listener != null)
            {
                lock (listener)
                {
                    listener.Stop();
                }
            }
            Log.WriteMessage("Finished.");
        }

        private void bw_Webserver_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }

        #endregion

    }

    public class WebserverDeliver : BackgroundWorker
    {
        string TmpDirectory = Application.StartupPath;

        public short GetElevation(double lat, double lon)
        {
            if (!GeographicalPoint.Check(lat, lon))
                return 0;
            short elv = ElevationData.Database.ElvMissingFlag;
            // try to get elevation data from distinct elevation model
            // start with detailed one
            if (Properties.Settings.Default.Elevation_SRTM1_Enabled && (elv == ElevationData.Database.ElvMissingFlag))
                elv = ElevationData.Database[lat, lon, ELEVATIONMODEL.SRTM1, false];
            if (Properties.Settings.Default.Elevation_SRTM3_Enabled && (elv == ElevationData.Database.ElvMissingFlag))
                elv = ElevationData.Database[lat, lon, ELEVATIONMODEL.SRTM3, false];
            if (Properties.Settings.Default.Elevation_GLOBE_Enabled && (elv == ElevationData.Database.ElvMissingFlag))
                elv = ElevationData.Database[lat, lon, ELEVATIONMODEL.GLOBE, false];
            // set it to zero if still invalid
            if (elv <= ElevationData.Database.TileMissingFlag)
                elv = 0;
            return elv;
        }

        public LocationDesignator LocationFind(string call, string loc = "", string bestcaseelevation = "")
        {
            // check all parameters
            if (!Callsign.Check(call))
                return null;
            if (!String.IsNullOrEmpty(loc) && !MaidenheadLocator.Check(loc))
                return null;

            bool best = false;
            if (!String.IsNullOrEmpty(bestcaseelevation))
            {
                // get the best case elevation switch if given
                best = bestcaseelevation.Trim().ToUpper() == "TRUE";
            }
            else
            {
                // use own Settings
                best = Properties.Settings.Default.Path_BestCaseElevation;
            }

            LocationDesignator ld = null;
            // loc empty --> get location info only from database
            if (String.IsNullOrEmpty(loc))
            {
                ld = StationData.Database.LocationFind(call);

                // return null if not found
                if (ld == null)
                    return null;
            }
            // loc set --> get location from database or from center of loc if not found
            else
            {
                ld = StationData.Database.LocationFind(call, loc);
                if (ld == null)
                {
                    ld = new LocationDesignator(call, loc, GEOSOURCE.FROMLOC);
                }
            }

            // get elevation
            ld.Elevation = GetElevation(ld.Lat, ld.Lon);
            ld.BestCaseElevation = false;

            // modify location in case of best case elevation is selected --> but do not store in database or settings!
            if (best)
            {
                if (!MaidenheadLocator.IsPrecise(ld.Lat, ld.Lon, 3))
                {
                    ElvMinMaxInfo maxinfo = GetMinMaxElevationLoc(ld.Loc);
                    if (maxinfo != null)
                    {
                        ld.Lat = maxinfo.MaxLat;
                        ld.Lon = maxinfo.MaxLon;
                        ld.Elevation = maxinfo.MaxElv;
                        ld.Source = GEOSOURCE.FROMBEST;
                        ld.BestCaseElevation = true;
                    }
                }
            }
            return ld;
        }

        public LocationDesignator LocationFindOrCreate(string call, string loc )
        {
            // check all parameters
            if (!Callsign.Check(call))
                return null;
            if (!MaidenheadLocator.Check(loc))
                return null;

            // get location info
            LocationDesignator ld = StationData.Database.LocationFindOrCreate(call, loc);
            // return null if not found/not valid
            if (ld == null)
                return null;

            // get elevation
            ld.Elevation = GetElevation(ld.Lat, ld.Lon);
            ld.BestCaseElevation = false;
            // modify location in case of best case elevation is selected --> but do not store in database or settings!
            if (Properties.Settings.Default.Path_BestCaseElevation)
            {
                if (!MaidenheadLocator.IsPrecise(ld.Lat, ld.Lon, 3))
                {
                    ElvMinMaxInfo maxinfo = GetMinMaxElevationLoc(ld.Loc);
                    if (maxinfo != null)
                    {
                        ld.Lat = maxinfo.MaxLat;
                        ld.Lon = maxinfo.MaxLon;
                        ld.Elevation = maxinfo.MaxElv;
                        ld.BestCaseElevation = true;
                    }
                }
            }
            return ld;
        }

        public List<LocationDesignator> LocationFindAll(string call, string bestcaseelevation = "")
        {
            // check all parameters
            if (!Callsign.Check(call))
                return null;

            bool best = false;
            if (!String.IsNullOrEmpty(bestcaseelevation))
            {
                // get the best case elevation switch if given
                best = bestcaseelevation.Trim().ToUpper() == "TRUE";
            }
            else
            {
                // use own Settings
                best = Properties.Settings.Default.Path_BestCaseElevation;
            }

            // get location info
            List<LocationDesignator> l = StationData.Database.LocationFindAll(call);

            // return null if not found
            if (l == null)
                return null;

            foreach (LocationDesignator ld in l)
            {
                // get elevation
                ld.Elevation = GetElevation(ld.Lat, ld.Lon);
                ld.BestCaseElevation = false;
                // modify location in case of best case elevation is selected --> but do not store in database or settings!
                if (best)
                {
                    if (!MaidenheadLocator.IsPrecise(ld.Lat, ld.Lon, 3))
                    {
                        ElvMinMaxInfo maxinfo = GetMinMaxElevationLoc(ld.Loc);
                        if (maxinfo != null)
                        {
                            ld.Lat = maxinfo.MaxLat;
                            ld.Lon = maxinfo.MaxLon;
                            ld.Elevation = maxinfo.MaxElv;
                            ld.Source = GEOSOURCE.FROMBEST;
                            ld.BestCaseElevation = true;
                        }
                    }
                }
            }
            return l;
        }

        public ElvMinMaxInfo GetMinMaxElevationLoc(string loc)
        {
            ElvMinMaxInfo elv = new ElvMinMaxInfo();
            // try to get elevation data from distinct elevation model
            // start with detailed one
            if (Properties.Settings.Default.Elevation_SRTM1_Enabled && (elv.MaxElv == ElevationData.Database.ElvMissingFlag))
            {
                ElvMinMaxInfo info = ElevationData.Database.GetMaxElvLoc(loc, ELEVATIONMODEL.SRTM1, false);
                if (info != null)
                {
                    elv.MaxLat = info.MaxLat;
                    elv.MaxLon = info.MaxLon;
                    elv.MaxElv = info.MaxElv;
                    elv.MinLat = info.MinLat;
                    elv.MinLon = info.MinLon;
                    elv.MinElv = info.MinElv;
                }
            }
            if (Properties.Settings.Default.Elevation_SRTM3_Enabled && (elv.MaxElv == ElevationData.Database.ElvMissingFlag))
            {
                ElvMinMaxInfo info = ElevationData.Database.GetMaxElvLoc(loc, ELEVATIONMODEL.SRTM3, false);
                if (info != null)
                {
                    elv.MaxLat = info.MaxLat;
                    elv.MaxLon = info.MaxLon;
                    elv.MaxElv = info.MaxElv;
                    elv.MinLat = info.MinLat;
                    elv.MinLon = info.MinLon;
                    elv.MinElv = info.MinElv;
                }
            }
            if (Properties.Settings.Default.Elevation_GLOBE_Enabled && (elv.MaxElv == ElevationData.Database.ElvMissingFlag))
            {
                ElvMinMaxInfo info = ElevationData.Database.GetMaxElvLoc(loc, ELEVATIONMODEL.GLOBE, false);
                if (info != null)
                {
                    elv.MaxLat = info.MaxLat;
                    elv.MaxLon = info.MaxLon;
                    elv.MaxElv = info.MaxElv;
                    elv.MinLat = info.MinLat;
                    elv.MinLon = info.MinLon;
                    elv.MinElv = info.MinElv;
                }
            }
            // set it to zero if still invalid
            if (elv.MaxElv == ElevationData.Database.ElvMissingFlag)
                elv.MaxElv = 0;
            if (elv.MinElv == ElevationData.Database.ElvMissingFlag)
                elv.MinElv = 0;

            return elv;
        }

        private string DeliverAircraftList(string tmpdir)
        {
            string json = "";
            var fs = File.OpenRead(tmpdir + Path.DirectorySeparatorChar + "vrsplanes.json");
            using (StreamReader sr = new StreamReader(fs))
            {
                json = sr.ReadToEnd();
            }
            return json;
        }

        private string DeliverPlanes(string tmpdir)
        {
            string json = "";
            var fs = File.OpenRead(tmpdir + Path.DirectorySeparatorChar + "planes.json");
            using (StreamReader sr = new StreamReader(fs))
            {
                json = sr.ReadToEnd();
            }
            return json;
        }

        private string DeliverVersion(string paramstr)
        {
            string version = Application.ProductVersion;
            string json = "";
            // convert path to json
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
            settings.FloatFormatHandling = FloatFormatHandling.String;
            settings.Formatting = Newtonsoft.Json.Formatting.Indented;
            json = JsonConvert.SerializeObject(version, settings);
            return json;
        }

        private string DeliverSettings(string paramstr)
        {
            //save settings
            Properties.Settings.Default.Save();
            string json = "";
            // convert path to json
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
            settings.FloatFormatHandling = FloatFormatHandling.String;
            settings.Formatting = Newtonsoft.Json.Formatting.Indented;
            json = JsonConvert.SerializeObject(Properties.Settings.Default, settings);
            return json;
        }
        private string DeliverBands(string paramstr)
        {
            string json = "";
            string[] bands = Bands.GetStringValuesExceptNoneAndAll();
            json = JsonConvert.SerializeObject(bands);
            return json;
        }
        private string DeliverBandValues(string paramstr)
        {
            string json = "";
            Band[] bands = Bands.GetBandsExceptNoneAndAll();
            json = JsonConvert.SerializeObject(bands);
            return json;
        }

        private string DeliverAircraftCategories(string paramstr)
        {
            string json = "";
            PlaneCategory[] cats = PlaneCategories.GetPlaneCategories();
            json = JsonConvert.SerializeObject(cats);
            return json;
        }

        private string DeliverLocation(string paramstr)
        {
            string json = "";
            // set default values
            string callstr = "";
            string locstr = "";
            string bestcasestr = "";

            // get parameters
            try
            {
                if (paramstr.Contains("?"))
                {
                    // OK, we have parameters --> cut them out and make all uppercase
                    paramstr = paramstr.Substring(paramstr.IndexOf("?") + 1).ToUpper();
                    var pars = System.Web.HttpUtility.ParseQueryString(paramstr);
                    callstr = pars.Get("CALL");
                    locstr = pars.Get("LOC");
                    bestcasestr = pars.Get("BESTCASEELEVATION");
                }
            }
            catch (Exception ex)
            {
                // return error
                return "Error while parsing parameters!";
            }

            // check parameters
            if (!String.IsNullOrEmpty(bestcasestr) && (bestcasestr != "TRUE") && (bestcasestr != "FALSE"))
            {
                return "Error: " + bestcasestr + " is not a valid value for best case elevation (true, false)!";
            }

            if (!Callsign.Check(callstr))
                return "Error: " + callstr + " is not a valid callsign!";
            LocationDesignator ld = null;

            // locstr == null or empty --> return last recent location
            if (String.IsNullOrEmpty(locstr))
            {
                ld = LocationFind(callstr, locstr, bestcasestr);
                json = ld.ToJSON();
                return json;
            }
            if(locstr == "ALL")
            {
                List<LocationDesignator> l = LocationFindAll(callstr, bestcasestr);
                JsonSerializerSettings settings = new JsonSerializerSettings();
                settings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                settings.FloatFormatHandling = FloatFormatHandling.String;
                settings.Formatting = Newtonsoft.Json.Formatting.Indented;
                settings.Culture = CultureInfo.InvariantCulture;
                json = JsonConvert.SerializeObject(l, settings);
                return json;
            }
            if (!MaidenheadLocator.Check(locstr))
                return "Error: " + locstr + " is not a valid Maidenhead locator!";
            // search call in station database, return empty string if not found
            ld = LocationFind(callstr, locstr, bestcasestr);
            json = ld.ToJSON();
            return json;
        }

        private string DeliverQRV(string paramstr)
        {
            string json = "";
            // set default values
            string callstr = "";
            string locstr = "";
            string bandstr = "";
            BAND band = Properties.Settings.Default.Band;
            // get parameters
            try
            {
                if (paramstr.Contains("?"))
                {
                    // OK, we have parameters --> cut them out and make all uppercase
                    paramstr = paramstr.Substring(paramstr.IndexOf("?") + 1).ToUpper();
                    var pars = System.Web.HttpUtility.ParseQueryString(paramstr);
                    callstr = pars.Get("CALL");
                    locstr = pars.Get("LOC");
                    bandstr = pars.Get("BAND");
                }
            }
            catch (Exception ex)
            {
                // return error
                return "Error while parsing parameters!";
            }
            // check parameters
            if (!Callsign.Check(callstr))
                return "Error: " + callstr + " is not a valid callsign!";
            if (!MaidenheadLocator.Check(locstr))
                return "Error: " + locstr + " is not a valid Maidenhead locator!";
            // set band to currently selected if empty
            if (string.IsNullOrEmpty(bandstr))
                band = Properties.Settings.Default.Band;
            else
                band = Bands.ParseStringValue(bandstr);
            if (band == BAND.BNONE)
                return "Error: " + bandstr + " is not a valid band value!";
            // search call in station database, return empty string if not found
            if (band != BAND.BALL)
            {
                QRVDesignator qrv = StationData.Database.QRVFind(callstr, locstr, band);
                if (qrv == null)
                    return "Error: QRV info not found in database!";
                json = qrv.ToJSON();
                return json;
            }
            List<QRVDesignator> qrvs = StationData.Database.QRVFind(callstr, locstr);
            if ((qrvs == null) || (qrvs.Count() == 0))
                return "Error: QRV info not found in database!";
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
            settings.FloatFormatHandling = FloatFormatHandling.String;
            settings.Formatting = Newtonsoft.Json.Formatting.Indented;
            json = JsonConvert.SerializeObject(qrvs, settings);
            return json;
        }

        private string DeliverElevationPath(string paramstr)
        {
            string json = "";

            // set default values
            string mycallstr = "";
            string mylatstr = "";
            string mylonstr = "";
            string mylocstr = "";
            string dxcallstr = "";
            string dxlatstr = "";
            string dxlonstr = "";
            string dxlocstr = "";

            double mylat = double.NaN;
            double mylon = double.NaN;
            double dxlat = double.NaN;
            double dxlon = double.NaN;

            LocationDesignator myloc = null;
            LocationDesignator dxloc = null;

            BAND band = Properties.Settings.Default.Band;

            // get parameters
            try
            {
                if (paramstr.Contains("?"))
                {
                    // OK, we have parameters --> cut them out and make all uppercase
                    paramstr = paramstr.Substring(paramstr.IndexOf("?") + 1).ToUpper();
                    var pars = System.Web.HttpUtility.ParseQueryString(paramstr);
                    mycallstr = pars.Get("MYCALL");
                    mylatstr = pars.Get("MYLAT");
                    mylonstr = pars.Get("MYLON");
                    mylocstr = pars.Get("MYLOC");
                    dxcallstr = pars.Get("DXCALL");
                    dxlatstr = pars.Get("DXLAT");
                    dxlonstr = pars.Get("DXLON");
                    dxlocstr = pars.Get("DXLOC");
                }
            }
            catch (Exception ex)
            {
                // return error
                return "Error while parsing parameters!";
            }
            // check parameters
            if (!Callsign.Check(mycallstr))
                return "Error: " + mycallstr + " is not a valid callsign!";
            if (!Callsign.Check(dxcallstr))
                return "Error: " + dxcallstr + " is not a valid callsign!";
            if (!String.IsNullOrEmpty(mylatstr) && !double.TryParse(mylatstr, NumberStyles.Float, CultureInfo.InvariantCulture, out mylat))
                return "Error: " + mylatstr + " is not a valid latitude!";
            if (!String.IsNullOrEmpty(mylonstr) && !double.TryParse(mylonstr, NumberStyles.Float, CultureInfo.InvariantCulture, out mylon))
                return "Error: " + mylonstr + " is not a valid longitude!";
            if (!String.IsNullOrEmpty(mylocstr) && !MaidenheadLocator.Check(mylocstr))
                return "Error: " + mylocstr + " is not a valid Maidenhead locator!";
            if (!String.IsNullOrEmpty(dxlatstr) && !double.TryParse(dxlatstr, NumberStyles.Float, CultureInfo.InvariantCulture, out dxlat))
                return "Error: " + dxlatstr + " is not a valid latitude!";
            if (!String.IsNullOrEmpty(dxlonstr) && !double.TryParse(dxlonstr, NumberStyles.Float, CultureInfo.InvariantCulture, out dxlon))
                return "Error: " + dxlonstr + " is not a valid longitude!";
            if (!String.IsNullOrEmpty(dxlocstr) && !MaidenheadLocator.Check(dxlocstr))
                return "Error: " + dxlocstr + " is not a valid Maidenhead locator!";

            // try to use database, if lat/lon not given
            if (double.IsNaN(mylat) || double.IsNaN(mylon))
            {
                // search call in station database, return empty string if not found
                myloc = LocationFind(mycallstr, mylocstr);
                if (myloc != null)
                {
                    mylat = myloc.Lat;
                    mylon = myloc.Lon;
                }
                else
                {
                    return "Error: MyLocation not found in database!";
                }
            }

            // set locator from lat/lon
            mylocstr = MaidenheadLocator.LocFromLatLon(mylat, mylon, false, 3);

            // try to use database, if lat/lon not given
            if (double.IsNaN(dxlat) || double.IsNaN(dxlon))
            {
                // search call in station database, return empty string if not found
                dxloc = LocationFind(dxcallstr, dxlocstr);
                if (dxloc != null)
                {
                    dxlat = dxloc.Lat;
                    dxlon = dxloc.Lon;
                }
                else
                {
                    return "Error: DXLocation not found in database!";
                }
            }

            // set locator from lat/lon
            dxlocstr = MaidenheadLocator.LocFromLatLon(dxlat, dxlon, false, 3);

            // get qrv info or create default
            QRVDesignator myqrv = StationData.Database.QRVFindOrCreateDefault(mycallstr, mylocstr, band);
            // set qrv defaults if zero
            if (myqrv.AntennaHeight == 0)
                myqrv.AntennaHeight = StationData.Database.QRVGetDefaultAntennaHeight(band);
            if (myqrv.AntennaGain == 0)
                myqrv.AntennaGain = StationData.Database.QRVGetDefaultAntennaGain(band);
            if (myqrv.Power == 0)
                myqrv.Power = StationData.Database.QRVGetDefaultPower(band);

            // get qrv info or create default
            QRVDesignator dxqrv = StationData.Database.QRVFindOrCreateDefault(dxcallstr, dxlocstr, band);
            // set qrv defaults if zero
            if (dxqrv.AntennaHeight == 0)
                dxqrv.AntennaHeight = StationData.Database.QRVGetDefaultAntennaHeight(band);
            if (dxqrv.AntennaGain == 0)
                dxqrv.AntennaGain = StationData.Database.QRVGetDefaultAntennaGain(band);
            if (dxqrv.Power == 0)
                dxqrv.Power = StationData.Database.QRVGetDefaultPower(band);

            // get or calculate elevation path
            ElevationPathDesignator epath = ElevationData.Database.ElevationPathFindOrCreateFromLatLon(
                this,
                mylat,
                mylon,
                dxlat,
                dxlon,
                ElevationData.Database.GetDefaultStepWidth(Properties.Settings.Default.ElevationModel),
                Properties.Settings.Default.ElevationModel);
            if (epath == null)
                return json;
            // add additional info to ppath
            epath.Location1 = myloc;
            epath.Location2 = dxloc;
            epath.QRV1 = myqrv;
            epath.QRV2 = dxqrv;
            // convert path to json
            json = epath.ToJSON();
            return json;
        }

        private string DeliverPropagationPath(string paramstr)
        {
            string json = "";
            // set default values
            string mycallstr = "";
            string mylatstr = "";
            string mylonstr = "";
            string mylocstr = "";
            string dxcallstr = "";
            string dxlatstr = "";
            string dxlonstr = "";
            string dxlocstr = "";

            double mylat = double.NaN;
            double mylon = double.NaN;
            double dxlat = double.NaN;
            double dxlon = double.NaN;

            LocationDesignator myloc = null;
            LocationDesignator dxloc = null;

            BAND band = Properties.Settings.Default.Band;

            // get parameters
            try
            {
                if (paramstr.Contains("?"))
                {
                    // OK, we have parameters --> cut them out and make all uppercase
                    paramstr = paramstr.Substring(paramstr.IndexOf("?") + 1).ToUpper();
                    var pars = System.Web.HttpUtility.ParseQueryString(paramstr);
                    mycallstr = pars.Get("MYCALL");
                    mylatstr = pars.Get("MYLAT");
                    mylonstr = pars.Get("MYLON");
                    mylocstr = pars.Get("MYLOC");
                    dxcallstr = pars.Get("DXCALL");
                    dxlatstr = pars.Get("DXLAT");
                    dxlonstr = pars.Get("DXLON");
                    dxlocstr = pars.Get("DXLOC");
                }
            }
            catch (Exception ex)
            {
                // return error
                return "Error while parsing parameters!";
            }
            // check parameters
            if (!Callsign.Check(mycallstr))
                return "Error: " + mycallstr + " is not a valid callsign!";
            if (!Callsign.Check(dxcallstr))
                return "Error: " + dxcallstr + " is not a valid callsign!";
            if (!String.IsNullOrEmpty(mylatstr) && !double.TryParse(mylatstr, NumberStyles.Float, CultureInfo.InvariantCulture, out mylat))
                return "Error: " + mylatstr + " is not a valid latitude!";
            if (!String.IsNullOrEmpty(mylonstr) && !double.TryParse(mylonstr, NumberStyles.Float, CultureInfo.InvariantCulture, out mylon))
                return "Error: " + mylonstr + " is not a valid longitude!";
            if (!String.IsNullOrEmpty(mylocstr) && !MaidenheadLocator.Check(mylocstr))
                return "Error: " + mylocstr + " is not a valid Maidenhead locator!";
            if (!String.IsNullOrEmpty(dxlatstr) && !double.TryParse(dxlatstr, NumberStyles.Float, CultureInfo.InvariantCulture, out dxlat))
                return "Error: " + dxlatstr + " is not a valid latitude!";
            if (!String.IsNullOrEmpty(dxlonstr) && !double.TryParse(dxlonstr, NumberStyles.Float, CultureInfo.InvariantCulture, out dxlon))
                return "Error: " + dxlonstr + " is not a valid longitude!";
            if (!String.IsNullOrEmpty(dxlocstr) && !MaidenheadLocator.Check(dxlocstr))
                return "Error: " + dxlocstr + " is not a valid Maidenhead locator!";

            // try to use database, if lat/lon not given
            if (double.IsNaN(mylat) || double.IsNaN(mylon))
            {
                // search call in station database, return empty string if not found
                myloc = LocationFind(mycallstr, mylocstr);
                if (myloc != null)
                {
                    mylat = myloc.Lat;
                    mylon = myloc.Lon;
                }
                else
                {
                    return "Error: MyLocation not found in database!";
                }
            }
            // set locator from lat/lon
            mylocstr = MaidenheadLocator.LocFromLatLon(mylat, mylon, false, 3);

            // get or create LocationDesignator with elevation info
            myloc = LocationFindOrCreate(mycallstr, mylocstr);

            // try to use database, if lat/lon not given
            if (double.IsNaN(dxlat) || double.IsNaN(dxlon))
            {
                // search call in station database, return empty string if not found
                dxloc = LocationFind(dxcallstr, dxlocstr);
                if (dxloc != null)
                {
                    dxlat = dxloc.Lat;
                    dxlon = dxloc.Lon;
                }
                else
                {
                    return "Error: DXLocation not found in database!";
                }
            }
            // set locator from lat/lon
            dxlocstr = MaidenheadLocator.LocFromLatLon(dxlat, dxlon, false, 3);

            // get or create LocationDesignator with elevation info
            dxloc = LocationFindOrCreate(dxcallstr, dxlocstr);

            // get qrv info or create default
            QRVDesignator myqrv = StationData.Database.QRVFindOrCreateDefault(mycallstr, mylocstr, band);
            // set qrv defaults if zero
            if (myqrv.AntennaHeight == 0)
                myqrv.AntennaHeight = StationData.Database.QRVGetDefaultAntennaHeight(band);
            if (myqrv.AntennaGain == 0)
                myqrv.AntennaGain = StationData.Database.QRVGetDefaultAntennaGain(band);
            if (myqrv.Power == 0)
                myqrv.Power = StationData.Database.QRVGetDefaultPower(band);

            // get qrv info or create default
            QRVDesignator dxqrv = StationData.Database.QRVFindOrCreateDefault(dxcallstr, dxlocstr, band);
            // set qrv defaults if zero
            if (dxqrv.AntennaHeight == 0)
                dxqrv.AntennaHeight = StationData.Database.QRVGetDefaultAntennaHeight(band);
            if (dxqrv.AntennaGain == 0)
                dxqrv.AntennaGain = StationData.Database.QRVGetDefaultAntennaGain(band);
            if (dxqrv.Power == 0)
                dxqrv.Power = StationData.Database.QRVGetDefaultPower(band);

            // find local obstruction, if any
            LocalObstructionDesignator o = ElevationData.Database.LocalObstructionFind(myloc.Lat, myloc.Lon, Properties.Settings.Default.ElevationModel);
            double mybearing = LatLon.Bearing(myloc.Lat, myloc.Lon, dxloc.Lat, dxloc.Lon);
            double myobstr = (o != null) ? o.GetObstruction(myqrv.AntennaHeight, mybearing) : double.MinValue;

            // get or calculate propagation path
            PropagationPathDesignator ppath = PropagationData.Database.PropagationPathFindOrCreateFromLatLon(
                this,
                myloc.Lat,
                myloc.Lon,
                myloc.Elevation + myqrv.AntennaHeight,
                dxloc.Lat,
                dxloc.Lon,
                dxloc.Elevation + dxqrv.AntennaHeight,
                Bands.ToGHz(band),
                LatLon.Earth.Radius * Properties.Settings.Default.Path_Band_Settings[band].K_Factor,
                Properties.Settings.Default.Path_Band_Settings[band].F1_Clearance,
                ElevationData.Database.GetDefaultStepWidth(Properties.Settings.Default.ElevationModel),
                Properties.Settings.Default.ElevationModel,
                myobstr);
            if (ppath == null)
                return json;
            // add additional info to ppath
            ppath.Location1 = myloc;
            ppath.Location2 = dxloc;
            ppath.QRV1 = myqrv;
            ppath.QRV2 = dxqrv;
            // convert path to json
            json = ppath.ToJSON();
            return json;
        }

        private string DeliverNearestPlanes(string paramstr, List<PlaneInfo> allplanes)
        {
            string json = "";
            // set default values
            string mycallstr = "";
            string mylocstr = "";
            string dxcallstr = "";
            string dxlocstr = "";
            string bandstr = "";
            BAND band = Properties.Settings.Default.Band;
            // get parameters
            try
            {
                if (paramstr.Contains("?"))
                {
                    // OK, we have parameters --> cut them out and make all uppercase
                    paramstr = paramstr.Substring(paramstr.IndexOf("?") + 1).ToUpper();
                    var pars = System.Web.HttpUtility.ParseQueryString(paramstr);
                    mycallstr = pars.Get("MYCALL");
                    mylocstr = pars.Get("MYLOC");
                    dxcallstr = pars.Get("DXCALL");
                    dxlocstr = pars.Get("DXLOC");
                    bandstr = pars.Get("BAND");
                }
            }
            catch (Exception ex)
            {
                // return error
                return "Error while parsing parameters!";
            }
            // check parameters
            if (!Callsign.Check(mycallstr))
                return "Error: " + mycallstr + " is not a valid callsign!";
            if (!Callsign.Check(dxcallstr))
                return "Error: " + dxcallstr + " is not a valid callsign!";
            if (!String.IsNullOrEmpty(mylocstr) && !MaidenheadLocator.Check(mylocstr))
                return "Error: " + mylocstr + " is not a valid Maidenhead locator!";
            if (!String.IsNullOrEmpty(dxlocstr) && !MaidenheadLocator.Check(dxlocstr))
                return "Error: " + dxlocstr + " is not a valid Maidenhead locator!";
            // set band to currently selected if empty
            if (string.IsNullOrEmpty(bandstr))
                band = Properties.Settings.Default.Band;
            else
                band = Bands.ParseStringValue(bandstr);
            if (band == BAND.BNONE)
                return "Error: " + bandstr + " is not a valid band value!";
            // search call in station database, return empty string if not found
            LocationDesignator myloc = LocationFind(mycallstr, mylocstr);
            if (myloc == null)
                return "Error: MyLocation not found in database!";
            LocationDesignator dxloc = LocationFind(dxcallstr, dxlocstr);
            if (dxloc == null)
                return "Error: DXLocation not found in database!";

            // get qrv info or create default
            QRVDesignator myqrv = StationData.Database.QRVFindOrCreateDefault(myloc.Call, myloc.Loc, band);
            // set qrv defaults if zero
            if (myqrv.AntennaHeight == 0)
                myqrv.AntennaHeight = StationData.Database.QRVGetDefaultAntennaHeight(band);
            if (myqrv.AntennaGain == 0)
                myqrv.AntennaGain = StationData.Database.QRVGetDefaultAntennaGain(band);
            if (myqrv.Power == 0)
                myqrv.Power = StationData.Database.QRVGetDefaultPower(band);

            // get qrv info or create default
            QRVDesignator dxqrv = StationData.Database.QRVFindOrCreateDefault(dxloc.Call, dxloc.Loc, band);
            // set qrv defaults if zero
            if (dxqrv.AntennaHeight == 0)
                dxqrv.AntennaHeight = StationData.Database.QRVGetDefaultAntennaHeight(band);
            if (dxqrv.AntennaGain == 0)
                dxqrv.AntennaGain = StationData.Database.QRVGetDefaultAntennaGain(band);
            if (dxqrv.Power == 0)
                dxqrv.Power = StationData.Database.QRVGetDefaultPower(band);

            // find local obstruction, if any
            LocalObstructionDesignator o = ElevationData.Database.LocalObstructionFind(myloc.Lat, myloc.Lon, Properties.Settings.Default.ElevationModel);
            double mybearing = LatLon.Bearing(myloc.Lat, myloc.Lon, dxloc.Lat, dxloc.Lon);
            double myobstr = (o != null) ? o.GetObstruction(myqrv.AntennaHeight, mybearing) : double.MinValue;

            // get or calculate propagation path
            PropagationPathDesignator ppath = PropagationData.Database.PropagationPathFindOrCreateFromLatLon(
                this,
                myloc.Lat,
                myloc.Lon,
                GetElevation(myloc.Lat, myloc.Lon) + myqrv.AntennaHeight,
                dxloc.Lat,
                dxloc.Lon,
                GetElevation(dxloc.Lat, dxloc.Lon) + dxqrv.AntennaHeight,
                Bands.ToGHz(band),
                LatLon.Earth.Radius * Properties.Settings.Default.Path_Band_Settings[band].K_Factor,
                Properties.Settings.Default.Path_Band_Settings[band].F1_Clearance,
                ElevationData.Database.GetDefaultStepWidth(Properties.Settings.Default.ElevationModel),
                Properties.Settings.Default.ElevationModel,
                myobstr);
            if (ppath == null)
                return json;
            // add additional info to ppath
            ppath.Location1 = myloc;
            ppath.Location2 = dxloc;
            ppath.QRV1 = myqrv;
            ppath.QRV2 = dxqrv;
            /*
            // estimate positions according to time
            DateTime time = DateTime.UtcNow;
            foreach(PlaneInfo plane in allplanes.Planes)
            {
                // change speed to km/h
                double speed = plane.Speed_kmh;
                // calculate distance after timespan
                double dist = speed * (time - allplanes.At).TotalHours;
                LatLon.GPoint newpos = LatLon.DestinationPoint(plane.Lat, plane.Lon, plane.Track, dist);
                plane.Lat = newpos.Lat;
                plane.Lon = newpos.Lon;
                plane.Time = time;
            }
            */
            // get nearest planes
            List<PlaneInfo> nearestplanes = AircraftData.Database.GetNearestPlanes(DateTime.UtcNow, ppath, allplanes, Properties.Settings.Default.Planes_Filter_Max_Circumcircle, Properties.Settings.Default.Path_Band_Settings[band].MaxDistance, Properties.Settings.Default.Planes_MaxAlt);
            // convert nearestplanes to json
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
            settings.FloatFormatHandling = FloatFormatHandling.String;
            settings.Formatting = Newtonsoft.Json.Formatting.Indented;
            json = JsonConvert.SerializeObject(nearestplanes, settings);
            return json;
        }

        private string DeliverAirports(string paramstr)
        {
            string json = "";
            List<AirportDesignator> airports = AircraftData.Database.AirportGetAll();
            json = JsonConvert.SerializeObject(airports);
            return json;
        }

        private string DeliverPlaneFeedSettings(WebServerDelivererArgs args, string paramstr)
        {
            string json = "";
            IPlaneFeedPlugin planefeed1 = null;
            IPlaneFeedPlugin planefeed2 = null;
            IPlaneFeedPlugin planefeed3 = null;

            // get parameters
            try
            {
                List<object> l = new List<object>();
                foreach (IPlaneFeedPlugin plugin in args.PlaneFeedPlugins)
                {
                    if (plugin.Name == Properties.Settings.Default.Planes_PlaneFeed1)
                        planefeed1 = plugin;
                    else if (plugin.Name == Properties.Settings.Default.Planes_PlaneFeed2)
                        planefeed2 = plugin;
                    else if (plugin.Name == Properties.Settings.Default.Planes_PlaneFeed3)
                        planefeed3 = plugin;
                }
                if (planefeed1 != null)
                {
                    KeyValuePair<string, object> feed = new KeyValuePair<string, object>(Properties.Settings.Default.Planes_PlaneFeed1, planefeed1.GetSettings());
                    l.Add(feed);
                }
                if (planefeed2 != null)
                {
                    KeyValuePair<string, object> feed = new KeyValuePair<string, object>(Properties.Settings.Default.Planes_PlaneFeed2, planefeed2.GetSettings());
                    l.Add(feed);
                }
                if (planefeed3 != null)
                {
                    KeyValuePair<string, object> feed = new KeyValuePair<string, object>(Properties.Settings.Default.Planes_PlaneFeed3, planefeed3.GetSettings());
                    l.Add(feed);
                }
                json = JsonConvert.SerializeObject(l);

                // convert to Name/Value
                json = json.Replace("\"Key\":", "\"Name\":");
            }
            catch (Exception ex)
            {
                // return error
                return "Error while parsing parameters!";
            }
            return json;
        }

        protected override void OnDoWork(DoWorkEventArgs e)
        {
            WebServerDelivererArgs args = (WebServerDelivererArgs)e.Argument;
            if (String.IsNullOrEmpty(Thread.CurrentThread.Name))
                Thread.CurrentThread.Name = this.GetType().Name + "_" + args.ID;

            byte[] buffer = new byte[0];
            string mime = "text/html";
            HttpStatusCode status = HttpStatusCode.OK;

            HttpListenerRequest request = args.Context.Request;

            // Obtain a response object.
            HttpListenerResponse response = args.Context.Response;

            // get unescaped raw url from request
            string rawurl = Uri.UnescapeDataString(request.RawUrl);

            // redirect parameterless calls to index.html
            if (!rawurl.Contains("/") || (rawurl == "/"))
            {
                rawurl = "/index.html";
            }

            // try to create local file name
            string filename = "";
            if (SupportFunctions.IsMono)
            {
                filename = rawurl.Substring(1);
            }
            else
            {
                filename = rawurl.Substring(1).Replace("/", "\\");
            }

            // cut parameters
            if (filename.Contains("?"))
                filename = filename.Substring(0, filename.IndexOf("?"));

            // try to find local file and deliver it
            filename = Path.Combine(args.WebserverDirectory, filename);
            if (File.Exists(filename))
            {
                buffer = File.ReadAllBytes(filename);
                mime = MimeTypeMap.GetMimeType(Path.GetExtension(filename));
            }
            // check for content delivery request
            else if (request.RawUrl == "/AircraftList.json")
            {
                buffer = System.Text.Encoding.UTF8.GetBytes(DeliverAircraftList(args.TmpDirectory));
                mime = "text/json";
            }
            // check for content delivery request
            else if (request.RawUrl.ToLower() == "/planes.json")
            {
                buffer = System.Text.Encoding.UTF8.GetBytes(DeliverPlanes(args.TmpDirectory));
                mime = "text/json";
            }
            else if (request.RawUrl.ToLower().StartsWith("/version.json"))
            {
                buffer = System.Text.Encoding.UTF8.GetBytes(DeliverVersion(request.RawUrl));
                mime = "text/json";
            }
            else if (request.RawUrl.ToLower().StartsWith("/settings.json"))
            {
                buffer = System.Text.Encoding.UTF8.GetBytes(DeliverSettings(request.RawUrl));
                mime = "text/json";
            }
            else if (request.RawUrl.ToLower().StartsWith("/bands.json"))
            {
                buffer = System.Text.Encoding.UTF8.GetBytes(DeliverBands(request.RawUrl));
                mime = "text/json";
            }
            else if (request.RawUrl.ToLower().StartsWith("/bandvalues.json"))
            {
                buffer = System.Text.Encoding.UTF8.GetBytes(DeliverBandValues(request.RawUrl));
                mime = "text/json";
            }
            else if (request.RawUrl.ToLower().StartsWith("/aircraftcategories.json"))
            {
                buffer = System.Text.Encoding.UTF8.GetBytes(DeliverAircraftCategories(request.RawUrl));
                mime = "text/json";
            }
            else if (request.RawUrl.ToLower().StartsWith("/location.json"))
            {
                buffer = System.Text.Encoding.UTF8.GetBytes(DeliverLocation(request.RawUrl));
                mime = "text/json";
            }
            else if (request.RawUrl.ToLower().StartsWith("/qrv.json"))
            {
                buffer = System.Text.Encoding.UTF8.GetBytes(DeliverQRV(request.RawUrl));
                mime = "text/json";
            }
            else if (request.RawUrl.ToLower().StartsWith("/elevationpath.json"))
            {
                buffer = System.Text.Encoding.UTF8.GetBytes(DeliverElevationPath(request.RawUrl));
                mime = "text/json";
            }
            else if (request.RawUrl.ToLower().StartsWith("/propagationpath.json"))
            {
                buffer = System.Text.Encoding.UTF8.GetBytes(DeliverPropagationPath(request.RawUrl));
                mime = "text/json";
            }
            else if (request.RawUrl.ToLower().StartsWith("/nearestplanes.json"))
            {
                buffer = System.Text.Encoding.UTF8.GetBytes(DeliverNearestPlanes(request.RawUrl, args.AllPlanes));
                mime = "text/json";
            }
            else if (request.RawUrl.ToLower().StartsWith("/airports.json"))
            {
                buffer = System.Text.Encoding.UTF8.GetBytes(DeliverAirports(request.RawUrl));
                mime = "text/json";
            }
            else if (request.RawUrl.ToLower().StartsWith("/planefeedsettings.json"))
            {
                buffer = System.Text.Encoding.UTF8.GetBytes(DeliverPlaneFeedSettings(args, request.RawUrl));
                mime = "text/json";
            }
            else
            {
                // mit Error 404 antworten
                buffer = System.Text.Encoding.UTF8.GetBytes("Not found!");
                status = HttpStatusCode.NotFound;
            }

            // Get a response stream and write the response to it.
            response.Headers.Add(HttpResponseHeader.CacheControl, "no-cache");
            response.ContentType = mime;
            response.StatusCode = (int)status;
            response.ContentLength64 = buffer.Length;
            System.IO.Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
//            Thread.Sleep(1000);
            // You must close the output stream.
            output.Close();
        }
    }

    public class WebServerDelivererArgs
    {
        public int ID;
        public string TmpDirectory = "";
        public string WebserverDirectory = "";
        public HttpListenerContext Context;
        public List<PlaneInfo> AllPlanes;
        public List<IPlaneFeedPlugin> PlaneFeedPlugins = new List<IPlaneFeedPlugin>();
    }
}
