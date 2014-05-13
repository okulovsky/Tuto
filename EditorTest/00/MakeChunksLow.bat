rmdir /s /q chunks
mkdir chunks
cd chunks
ffmpeg -i ..\face-converted.avi -ss 38.089 -t 9.478 -acodec copy -vcodec copy chunk004.avi
ffmpeg -i ..\face-converted.avi -ss 55.678 -t 43.310 -acodec copy -vcodec copy chunk007.avi
ffmpeg -i ..\face-converted.avi -ss 104.395 -t 13.487 -acodec copy -vcodec copy chunk010.avi
ffmpeg -i ..\face-converted.avi -ss 131.068 -t 6.546 -acodec copy -vcodec copy chunk013.avi
ffmpeg -i ..\face-converted.avi -ss 139.509 -t 3.872 -acodec copy -vcodec copy chunk016.avi
ffmpeg -i ..\face-converted.avi -ss 151.524 -t 13.469 -acodec copy -vcodec copy chunk019.avi
ffmpeg -i ..\face-converted.avi -ss 171.487 -t 3.814 -acodec copy -vcodec copy chunk022.avi
ffmpeg -i ..\face-converted.avi -ss 179.573 -t 10.883 -acodec copy -vcodec copy chunk025.avi
ffmpeg -i ..\face-converted.avi -ss 216.547 -t 10.822 -acodec copy -vcodec copy chunk028.avi
ffmpeg -i ..\face-converted.avi -ss 245.157 -t 38.904 -acodec copy -vcodec copy chunk031.avi
ffmpeg -i ..\face-converted.avi -ss 297.388 -t 6.297 -acodec copy -vcodec copy chunk034.avi
ffmpeg -i ..\face-converted.avi -ss 335.662 -t 14.432 -acodec copy -vn audio037.avi
ffmpeg -i ..\desktop-converted.avi -ss 321.039 -t 14.432 -acodec copy -vcodec copy video037.avi
ffmpeg -i audio037.avi -i video037.avi -acodec copy -vcodec copy chunk037.avi
ffmpeg -i ..\face-converted.avi -ss 352.721 -t 19.755 -acodec copy -vcodec copy chunk040.avi
ffmpeg -i ..\face-converted.avi -ss 375.257 -t 37.331 -acodec copy -vcodec copy chunk043.avi
ffmpeg -i ..\face-converted.avi -ss 414.573 -t 70.826 -acodec copy -vn audio046.avi
ffmpeg -i ..\desktop-converted.avi -ss 399.950 -t 70.826 -acodec copy -vcodec copy video046.avi
ffmpeg -i audio046.avi -i video046.avi -acodec copy -vcodec copy chunk046.avi
ffmpeg -i ..\face-converted.avi -ss 488.186 -t 9.179 -acodec copy -vn audio049.avi
ffmpeg -i ..\desktop-converted.avi -ss 473.563 -t 9.179 -acodec copy -vcodec copy video049.avi
ffmpeg -i audio049.avi -i video049.avi -acodec copy -vcodec copy chunk049.avi
ffmpeg -i ..\face-converted.avi -ss 500.873 -t 4.289 -acodec copy -vcodec copy chunk052.avi
ffmpeg -i ..\face-converted.avi -ss 509.913 -t 8.406 -acodec copy -vcodec copy chunk055.avi
ffmpeg -i ..\face-converted.avi -ss 523.864 -t 10.109 -acodec copy -vcodec copy chunk058.avi
ffmpeg -i ..\face-converted.avi -ss 536.074 -t 8.259 -acodec copy -vcodec copy chunk061.avi
ffmpeg -i ..\face-converted.avi -ss 544.333 -t 3.384 -acodec copy -vn audio063.avi
ffmpeg -i ..\desktop-converted.avi -ss 529.710 -t 3.384 -acodec copy -vcodec copy video063.avi
ffmpeg -i audio063.avi -i video063.avi -acodec copy -vcodec copy chunk063.avi
ffmpeg -i ..\face-converted.avi -ss 547.717 -t 11.059 -acodec copy -vcodec copy chunk065.avi
ffmpeg -i ..\face-converted.avi -ss 566.033 -t 34.069 -acodec copy -vn audio068.avi
ffmpeg -i ..\desktop-converted.avi -ss 551.410 -t 34.069 -acodec copy -vcodec copy video068.avi
ffmpeg -i audio068.avi -i video068.avi -acodec copy -vcodec copy chunk068.avi
ffmpeg -i ..\face-converted.avi -ss 600.102 -t 11.601 -acodec copy -vcodec copy chunk070.avi
ffmpeg -i ..\face-converted.avi -ss 611.703 -t 10.624 -acodec copy -vn audio072.avi
ffmpeg -i ..\desktop-converted.avi -ss 597.080 -t 10.624 -acodec copy -vcodec copy video072.avi
ffmpeg -i audio072.avi -i video072.avi -acodec copy -vcodec copy chunk072.avi
ffmpeg -i ..\face-converted.avi -ss 632.680 -t 20.205 -acodec copy -vcodec copy chunk075.avi
ffmpeg -i ..\face-converted.avi -ss 664.930 -t 15.762 -acodec copy -vcodec copy chunk078.avi
ffmpeg -i ..\face-converted.avi -ss 688.729 -t 10.126 -acodec copy -vcodec copy chunk081.avi
ffmpeg -i ..\face-converted.avi -ss 698.855 -t 13.403 -acodec copy -vn audio083.avi
ffmpeg -i ..\desktop-converted.avi -ss 684.232 -t 13.403 -acodec copy -vcodec copy video083.avi
ffmpeg -i audio083.avi -i video083.avi -acodec copy -vcodec copy chunk083.avi
ffmpeg -i ..\face-converted.avi -ss 716.805 -t 4.350 -acodec copy -vn audio086.avi
ffmpeg -i ..\desktop-converted.avi -ss 702.182 -t 4.350 -acodec copy -vcodec copy video086.avi
ffmpeg -i audio086.avi -i video086.avi -acodec copy -vcodec copy chunk086.avi
ffmpeg -i ..\face-converted.avi -ss 733.437 -t 13.320 -acodec copy -vn audio089.avi
ffmpeg -i ..\desktop-converted.avi -ss 718.814 -t 13.320 -acodec copy -vcodec copy video089.avi
ffmpeg -i audio089.avi -i video089.avi -acodec copy -vcodec copy chunk089.avi
ffmpeg -i ..\face-converted.avi -ss 752.271 -t 8.724 -acodec copy -vn audio092.avi
ffmpeg -i ..\desktop-converted.avi -ss 737.648 -t 8.724 -acodec copy -vcodec copy video092.avi
ffmpeg -i audio092.avi -i video092.avi -acodec copy -vcodec copy chunk092.avi
ffmpeg -i ..\face-converted.avi -ss 782.951 -t 7.536 -acodec copy -vn audio095.avi
ffmpeg -i ..\desktop-converted.avi -ss 768.328 -t 7.536 -acodec copy -vcodec copy video095.avi
ffmpeg -i audio095.avi -i video095.avi -acodec copy -vcodec copy chunk095.avi
ffmpeg -i ..\face-converted.avi -ss 796.061 -t 60.521 -acodec copy -vn audio098.avi
ffmpeg -i ..\desktop-converted.avi -ss 781.438 -t 60.521 -acodec copy -vcodec copy video098.avi
ffmpeg -i audio098.avi -i video098.avi -acodec copy -vcodec copy chunk098.avi
ffmpeg -i ..\face-converted.avi -ss 866.954 -t 29.380 -acodec copy -vn audio101.avi
ffmpeg -i ..\desktop-converted.avi -ss 852.331 -t 29.380 -acodec copy -vcodec copy video101.avi
ffmpeg -i audio101.avi -i video101.avi -acodec copy -vcodec copy chunk101.avi
ffmpeg -i ..\face-converted.avi -ss 903.519 -t 5.281 -acodec copy -vn audio104.avi
ffmpeg -i ..\desktop-converted.avi -ss 888.896 -t 5.281 -acodec copy -vcodec copy video104.avi
ffmpeg -i audio104.avi -i video104.avi -acodec copy -vcodec copy chunk104.avi
ffmpeg -i ..\face-converted.avi -ss 913.904 -t 52.397 -acodec copy -vcodec copy chunk107.avi
ffmpeg -i ..\face-converted.avi -ss 981.164 -t 6.278 -acodec copy -vcodec copy chunk110.avi
ffmpeg -i ..\face-converted.avi -ss 991.731 -t 6.148 -acodec copy -vcodec copy chunk113.avi
ffmpeg -i ..\face-converted.avi -ss 1037.885 -t 16.473 -acodec copy -vcodec copy chunk117.avi
ffmpeg -i ..\face-converted.avi -ss 1076.742 -t 19.390 -acodec copy -vcodec copy chunk120.avi
ffmpeg -i ..\face-converted.avi -ss 1113.627 -t 18.958 -acodec copy -vcodec copy chunk123.avi
ffmpeg -i ..\face-converted.avi -ss 1132.585 -t 32.576 -acodec copy -vn audio125.avi
ffmpeg -i ..\desktop-converted.avi -ss 1117.962 -t 32.576 -acodec copy -vcodec copy video125.avi
ffmpeg -i audio125.avi -i video125.avi -acodec copy -vcodec copy chunk125.avi
ffmpeg -i ..\face-converted.avi -ss 1165.161 -t 7.980 -acodec copy -vcodec copy chunk127.avi
ffmpeg -i ..\face-converted.avi -ss 1173.141 -t 7.717 -acodec copy -vn audio129.avi
ffmpeg -i ..\desktop-converted.avi -ss 1158.518 -t 7.717 -acodec copy -vcodec copy video129.avi
ffmpeg -i audio129.avi -i video129.avi -acodec copy -vcodec copy chunk129.avi
ffmpeg -i ..\face-converted.avi -ss 1196.725 -t 7.383 -acodec copy -vcodec copy chunk132.avi
ffmpeg -i ..\face-converted.avi -ss 1215.560 -t 6.671 -acodec copy -vcodec copy chunk135.avi
ffmpeg -i ..\face-converted.avi -ss 1225.802 -t 38.010 -acodec copy -vcodec copy chunk138.avi
ffmpeg -i ..\face-converted.avi -ss 1279.118 -t 18.222 -acodec copy -vcodec copy chunk141.avi
ffmpeg -i ..\face-converted.avi -ss 1301.588 -t 55.800 -acodec copy -vcodec copy chunk144.avi
ffmpeg -i ..\face-converted.avi -ss 1405.515 -t 8.032 -acodec copy -vn audio147.avi
ffmpeg -i ..\desktop-converted.avi -ss 1390.892 -t 8.032 -acodec copy -vcodec copy video147.avi
ffmpeg -i audio147.avi -i video147.avi -acodec copy -vcodec copy chunk147.avi
ffmpeg -i ..\face-converted.avi -ss 1413.547 -t 28.045 -acodec copy -vcodec copy chunk149.avi
ffmpeg -i ..\face-converted.avi -ss 1444.925 -t 4.694 -acodec copy -vcodec copy chunk152.avi
ffmpeg -i ..\face-converted.avi -ss 1465.662 -t 17.751 -acodec copy -vn audio155.avi
ffmpeg -i ..\desktop-converted.avi -ss 1451.039 -t 17.751 -acodec copy -vcodec copy video155.avi
ffmpeg -i audio155.avi -i video155.avi -acodec copy -vcodec copy chunk155.avi
ffmpeg -i ..\face-converted.avi -ss 1501.589 -t 16.484 -acodec copy -vn audio158.avi
ffmpeg -i ..\desktop-converted.avi -ss 1486.966 -t 16.484 -acodec copy -vcodec copy video158.avi
ffmpeg -i audio158.avi -i video158.avi -acodec copy -vcodec copy chunk158.avi
ffmpeg -i ..\face-converted.avi -ss 1521.866 -t 48.824 -acodec copy -vn audio161.avi
ffmpeg -i ..\desktop-converted.avi -ss 1507.243 -t 48.824 -acodec copy -vcodec copy video161.avi
ffmpeg -i audio161.avi -i video161.avi -acodec copy -vcodec copy chunk161.avi
ffmpeg -i ..\face-converted.avi -ss 1585.900 -t 71.115 -acodec copy -vcodec copy chunk164.avi
ffmpeg -i ..\face-converted.avi -ss 1657.015 -t 4.628 -acodec copy -vn audio166.avi
ffmpeg -i ..\desktop-converted.avi -ss 1642.392 -t 4.628 -acodec copy -vcodec copy video166.avi
ffmpeg -i audio166.avi -i video166.avi -acodec copy -vcodec copy chunk166.avi
ffmpeg -i ..\face-converted.avi -ss 1684.087 -t 49.324 -acodec copy -vn audio169.avi
ffmpeg -i ..\desktop-converted.avi -ss 1669.464 -t 49.324 -acodec copy -vcodec copy video169.avi
ffmpeg -i audio169.avi -i video169.avi -acodec copy -vcodec copy chunk169.avi
ffmpeg -i ..\face-converted.avi -ss 1736.388 -t 58.425 -acodec copy -vn audio172.avi
ffmpeg -i ..\desktop-converted.avi -ss 1721.765 -t 58.425 -acodec copy -vcodec copy video172.avi
ffmpeg -i audio172.avi -i video172.avi -acodec copy -vcodec copy chunk172.avi
ffmpeg -i ..\face-converted.avi -ss 1797.934 -t 12.183 -acodec copy -vn audio175.avi
ffmpeg -i ..\desktop-converted.avi -ss 1783.311 -t 12.183 -acodec copy -vcodec copy video175.avi
ffmpeg -i audio175.avi -i video175.avi -acodec copy -vcodec copy chunk175.avi
ffmpeg -i ..\face-converted.avi -ss 1811.445 -t 13.315 -acodec copy -vn audio178.avi
ffmpeg -i ..\desktop-converted.avi -ss 1796.822 -t 13.315 -acodec copy -vcodec copy video178.avi
ffmpeg -i audio178.avi -i video178.avi -acodec copy -vcodec copy chunk178.avi
ffmpeg -i ..\face-converted.avi -ss 1833.832 -t 19.623 -acodec copy -vn audio181.avi
ffmpeg -i ..\desktop-converted.avi -ss 1819.209 -t 19.623 -acodec copy -vcodec copy video181.avi
ffmpeg -i audio181.avi -i video181.avi -acodec copy -vcodec copy chunk181.avi
ffmpeg -i ..\face-converted.avi -ss 1861.922 -t 7.091 -acodec copy -vn audio184.avi
ffmpeg -i ..\desktop-converted.avi -ss 1847.299 -t 7.091 -acodec copy -vcodec copy video184.avi
ffmpeg -i audio184.avi -i video184.avi -acodec copy -vcodec copy chunk184.avi
ffmpeg -i ..\face-converted.avi -ss 1872.524 -t 22.248 -acodec copy -vn audio187.avi
ffmpeg -i ..\desktop-converted.avi -ss 1857.901 -t 22.248 -acodec copy -vcodec copy video187.avi
ffmpeg -i audio187.avi -i video187.avi -acodec copy -vcodec copy chunk187.avi
ffmpeg -i ..\face-converted.avi -ss 1898.442 -t 59.077 -acodec copy -vcodec copy chunk190.avi
ffmpeg -i ..\face-converted.avi -ss 1991.922 -t 27.212 -acodec copy -vcodec copy chunk193.avi
ffmpeg -i ..\face-converted.avi -ss 2073.362 -t 14.062 -acodec copy -vcodec copy chunk197.avi
ffmpeg -i ..\face-converted.avi -ss 2135.705 -t 73.039 -acodec copy -vcodec copy chunk200.avi
ffmpeg -i ..\face-converted.avi -ss 2221.923 -t 50.898 -acodec copy -vcodec copy chunk203.avi
ffmpeg -i ..\face-converted.avi -ss 2272.821 -t 2.696 -acodec copy -vn audio205.avi
ffmpeg -i ..\desktop-converted.avi -ss 2258.198 -t 2.696 -acodec copy -vcodec copy video205.avi
ffmpeg -i audio205.avi -i video205.avi -acodec copy -vcodec copy chunk205.avi
ffmpeg -i ..\face-converted.avi -ss 2283.173 -t 81.925 -acodec copy -vn audio208.avi
ffmpeg -i ..\desktop-converted.avi -ss 2268.550 -t 81.925 -acodec copy -vcodec copy video208.avi
ffmpeg -i audio208.avi -i video208.avi -acodec copy -vcodec copy chunk208.avi
ffmpeg -i ..\face-converted.avi -ss 2365.098 -t 11.652 -acodec copy -vcodec copy chunk210.avi
ffmpeg -i ..\face-converted.avi -ss 2376.750 -t 63.546 -acodec copy -vn audio212.avi
ffmpeg -i ..\desktop-converted.avi -ss 2362.127 -t 63.546 -acodec copy -vcodec copy video212.avi
ffmpeg -i audio212.avi -i video212.avi -acodec copy -vcodec copy chunk212.avi
ffmpeg -i ..\face-converted.avi -ss 2463.093 -t 9.726 -acodec copy -vn audio215.avi
ffmpeg -i ..\desktop-converted.avi -ss 2448.470 -t 9.726 -acodec copy -vcodec copy video215.avi
ffmpeg -i audio215.avi -i video215.avi -acodec copy -vcodec copy chunk215.avi
ffmpeg -i ..\face-converted.avi -ss 2487.952 -t 19.614 -acodec copy -vn audio218.avi
ffmpeg -i ..\desktop-converted.avi -ss 2473.329 -t 19.614 -acodec copy -vcodec copy video218.avi
ffmpeg -i audio218.avi -i video218.avi -acodec copy -vcodec copy chunk218.avi
ffmpeg -i ..\face-converted.avi -ss 2507.566 -t 16.950 -acodec copy -vcodec copy chunk220.avi
ffmpeg -i ..\face-converted.avi -ss 2524.516 -t 10.516 -acodec copy -vn audio222.avi
ffmpeg -i ..\desktop-converted.avi -ss 2509.893 -t 10.516 -acodec copy -vcodec copy video222.avi
ffmpeg -i audio222.avi -i video222.avi -acodec copy -vcodec copy chunk222.avi
ffmpeg -i ..\face-converted.avi -ss 2558.219 -t 23.175 -acodec copy -vn audio225.avi
ffmpeg -i ..\desktop-converted.avi -ss 2543.596 -t 23.175 -acodec copy -vcodec copy video225.avi
ffmpeg -i audio225.avi -i video225.avi -acodec copy -vcodec copy chunk225.avi
ffmpeg -i ..\face-converted.avi -ss 2583.450 -t 23.030 -acodec copy -vn audio228.avi
ffmpeg -i ..\desktop-converted.avi -ss 2568.827 -t 23.030 -acodec copy -vcodec copy video228.avi
ffmpeg -i audio228.avi -i video228.avi -acodec copy -vcodec copy chunk228.avi
ffmpeg -i ..\face-converted.avi -ss 2610.219 -t 3.083 -acodec copy -vcodec copy chunk231.avi
ffmpeg -i ..\face-converted.avi -ss 2619.004 -t 17.015 -acodec copy -vcodec copy chunk234.avi
ffmpeg -i ..\face-converted.avi -ss 2649.818 -t 19.613 -acodec copy -vcodec copy chunk237.avi
cd ..
