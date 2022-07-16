using HR.BrightspaceConnector.Utilities;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HR.BrightspaceConnector.Tests
{
    [TestClass]
    public class ElegantPairingFunctionsTests
    {
        [DataTestMethod]
        [DataRow(2869, 1646, 8235676)]
        [DataRow(1041, 457, 1085179)]
        [DataRow(1387, 937, 1926093)]
        [DataRow(2833, 2806, 8031528)]
        [DataRow(532, 744, 554068)]
        [DataRow(77, 2306, 5317713)]
        [DataRow(2920, 2248, 8531568)]
        [DataRow(1827, 219, 3339975)]
        [DataRow(835, 2839, 8060756)]
        [DataRow(991, 649, 983721)]
        [DataRow(2769, 1315, 7671445)]
        [DataRow(1442, 2912, 8481186)]
        [DataRow(1831, 747, 3355139)]
        [DataRow(1919, 665, 3685145)]
        [DataRow(2960, 1661, 8766221)]
        [DataRow(2273, 1173, 5169975)]
        [DataRow(2139, 755, 4578215)]
        [DataRow(1246, 2015, 4061471)]
        [DataRow(2614, 2917, 8511503)]
        [DataRow(1591, 2029, 4118432)]
        [DataRow(779, 2423, 5871708)]
        [DataRow(462, 247, 214153)]
        [DataRow(1799, 2368, 5609223)]
        [DataRow(899, 2459, 6047580)]
        [DataRow(1596, 1727, 2984125)]
        [DataRow(2394, 1360, 5734990)]
        [DataRow(2694, 2428, 7262758)]
        [DataRow(452, 548, 300756)]
        [DataRow(2798, 1516, 7833118)]
        [DataRow(396, 2987, 8922565)]
        [DataRow(2000, 2438, 5945844)]
        [DataRow(1342, 2359, 5566223)]
        [DataRow(1384, 1929, 3722425)]
        [DataRow(1322, 625, 1749631)]
        [DataRow(1965, 389, 3863579)]
        [DataRow(210, 2220, 4928610)]
        [DataRow(1108, 2915, 8498333)]
        [DataRow(60, 1821, 3316101)]
        [DataRow(373, 1325, 1755998)]
        [DataRow(177, 283, 80266)]
        [DataRow(1512, 175, 2287831)]
        [DataRow(2137, 510, 4569416)]
        [DataRow(1791, 294, 3209766)]
        [DataRow(2037, 223, 4151629)]
        [DataRow(555, 824, 679531)]
        [DataRow(2180, 1049, 4755629)]
        [DataRow(145, 789, 622666)]
        [DataRow(1291, 478, 1668450)]
        [DataRow(2382, 987, 5677293)]
        [DataRow(618, 2500, 6250618)]
        [DataRow(1041, 1267, 1606330)]
        [DataRow(1681, 2572, 6616865)]
        [DataRow(2745, 1183, 7538953)]
        [DataRow(2468, 575, 6094067)]
        [DataRow(2234, 779, 4993769)]
        [DataRow(898, 6, 807308)]
        [DataRow(769, 1520, 2311169)]
        [DataRow(895, 1096, 1202111)]
        [DataRow(1662, 295, 2764201)]
        [DataRow(2936, 117, 8623149)]
        [DataRow(1515, 2768, 7663339)]
        [DataRow(1218, 2086, 4352614)]
        [DataRow(2616, 1396, 6847468)]
        [DataRow(2021, 979, 4087441)]
        [DataRow(714, 732, 536538)]
        [DataRow(1492, 1302, 2228858)]
        [DataRow(890, 701, 793691)]
        [DataRow(2611, 2278, 6822210)]
        [DataRow(1334, 1891, 3577215)]
        [DataRow(2522, 489, 6363495)]
        [DataRow(2391, 944, 5720216)]
        [DataRow(2189, 2998, 8990193)]
        [DataRow(770, 1487, 2211939)]
        [DataRow(1021, 272, 1043734)]
        [DataRow(790, 401, 625291)]
        [DataRow(1888, 628, 3567060)]
        [DataRow(572, 2380, 5664972)]
        [DataRow(2307, 1813, 5326369)]
        [DataRow(1680, 618, 2824698)]
        [DataRow(607, 429, 369485)]
        [DataRow(560, 2295, 5267585)]
        [DataRow(2337, 1895, 5465801)]
        [DataRow(526, 1703, 2900735)]
        [DataRow(259, 53, 67393)]
        [DataRow(780, 809, 655261)]
        [DataRow(2854, 1023, 8149193)]
        [DataRow(2113, 1402, 4468284)]
        [DataRow(581, 664, 441477)]
        [DataRow(245, 1270, 1613145)]
        [DataRow(2194, 2176, 4818006)]
        [DataRow(1003, 2312, 5346347)]
        [DataRow(2570, 1377, 6608847)]
        [DataRow(856, 1128, 1273240)]
        [DataRow(1886, 252, 3559134)]
        [DataRow(604, 2963, 8779973)]
        [DataRow(2966, 737, 8800859)]
        [DataRow(1373, 1302, 1887804)]
        [DataRow(2595, 2631, 6924756)]
        [DataRow(476, 1092, 1192940)]
        [DataRow(894, 710, 800840)]
        public void PairAndUnpair_ReturnsOriginalInput(int x, int y, long z)
        {
            var result = ElegantPairingFunctions.Pair((uint)x, (uint)y);

            Assert.AreEqual(z, (long)result);

            ElegantPairingFunctions.Unpair(result, out var a, out var b);

            Assert.AreEqual(x, (int)a);
            Assert.AreEqual(y, (int)b);
        }

        [TestMethod]
        public void PairAndUnpair_ReturnsOriginalInput()
        {
            const int lowerBound = 0;
            const int upperBound = 1024;

            int x = lowerBound;
            int y = upperBound;

            while (x <= upperBound && y >= lowerBound)
            {
                var z = ElegantPairingFunctions.Pair((uint)x, (uint)y);

                ElegantPairingFunctions.Unpair(z, out var a, out var b);

                Assert.AreEqual(x, (int)a);
                Assert.AreEqual(y, (int)b);

                x++;
                y--;
            }
        }
    }
}
