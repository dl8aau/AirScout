using ScoutBase.Core;
using ScoutBase.Database;
using ScoutBase.Propagation;
using AirScout.Core;
using System.Security.Policy;
using System.Numerics;
using AirScout.Aircrafts;

namespace TestClass.TestGetNearestPlanes
{
    [TestClass]
    public sealed class Test1
    {
        [TestMethod]
        public void TestGetNearestPlanes1()
        {
            // DK0MM - M0GHZ 432MHz
            //public PropagationPathDesignator(double lat1, double lon1, double h1,
            //                                 double lat2, double lon2, double h2, double qrg, double radius, double f1_clearance, double stepwidth, double eps1_min, double eps2_min, double localobstruction)
            PropagationPathDesignator ppath = new PropagationPathDesignator(49.839298248291, 8.74973964691162, 292, // double lat1, double lon1, double h1
                51.4575, -2.25, 169,    // double lat2, double lon2, double h2
                0.432, 8919.4, 0.4, 90, // double qrg, double radius, double f1_clearance, double stepwidth
                    0.0108990218637874, 0.00377353585918658, double.MinValue // double eps1_min, double eps2_min, double localobstruction
                    );

            // DK0MM - M0GHZ 432MHz
            //public PropagationPathDesignator(double lat1, double lon1, double h1,
            //                                 double lat2, double lon2, double h2, double qrg, double radius, double f1_clearance, double stepwidth, double eps1_min, double eps2_min, double localobstruction)
            PropagationPathDesignator ppath2 = new PropagationPathDesignator(49.839298248291, 8.74973964691162, 292,
                52.0758333333333, 1.25166666666667, 46,
                0.432, 8919.4, 0.4, 90,
                0.00225245910188553, 0.00188331712327414, double.MinValue);

            PlaneInfo plane = new PlaneInfo(DateTime.Parse("04 / 01 / 2025 16:25:25"), "RYR34ED", "[unknown]", "4CADF4", 50.1379391778383, 2.91418011012365, 42.4000015258789, 22025, 348, "[unknown]", "Embraer", "EMB - 190 E2", PLANECATEGORY.NONE);
            PlaneInfo plane2 = new PlaneInfo(DateTime.Parse("04 / 02 / 2025 08:35:05"), "TAP640", "[unknown]", "4952CE", 49.8479850024046,2.41892344449693,43.630328331166, 31134.1941242742, 334.729042130416, "[unknown]", "Embraer", "EMB-190 E2", PLANECATEGORY.NONE);
            PlaneInfo plane3 = new PlaneInfo(DateTime.Parse("04/02/2025 08:39:05"), "SAS22E", "[unknown]", "4ACAAF", 51.5125204724554,4.33224249666312,205.904300871498, 38072.8466439235, 482.827731513877, "[unknown]", "Embraer", "EMB-190 E2", PLANECATEGORY.NONE);

 

            AircraftData.Database.ComputePlanePotential(ref plane, ppath, 10, 12200);
            AircraftData.Database.ComputePlanePotential(ref plane2, ppath, 10, 12200);
            AircraftData.Database.ComputePlanePotential(ref plane3, ppath, 10, 12200);

            //GB0MHL
            PlaneInfo plane4 = new PlaneInfo(DateTime.Parse("04/03/2025 13:43:06"), "DHK515", "[unknown]", "407C8A", 50.6121109357803, 6.27786054958822, 289.837951185701, 36000, 479.759221515438, "[unknown]", "Embraer", "EMB-190 E2", PLANECATEGORY.NONE);
            PlaneInfo plane5 = new PlaneInfo(DateTime.Parse("04/04/2025 15:54:20"), "THY81D", "TC-LGC", "4BB0E3", 50.5545318238118,6.75755836594228,290.357503969874, 40069.0471843181, 486.681320687762, "A359", "AIRBUS", "A-350-900/A-350-900 XWB/A-350-900 XWB Prestige/Prestige (A-350-900)", PLANECATEGORY.HEAVY);

            AircraftData.Database.ComputePlanePotential(ref plane4, ppath2, 10, 12200);

        }
    }
}
