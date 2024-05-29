const baseURL = 'ip';
export const imageURL = baseURL;
const testImage = 'http://[ip]:6533/images/01301172602430_WTF5456_vehicle.jpg';
const testSiteImage =
  'http://[ip]:6533/images/01301172602430_WTF5456_vehicle.jpg';
const linuxImage =
  'http://0.0.0.0/anpr/data/plate_img/2023-03-29/ANPR-VMS13_01_20230329-124018_29-03-2023_03-5_01301124000150_BKW7887.jpg';

//Detection SignalR
export const detection = baseURL + '/detection';

//License Plate Recognition
export const historyDetection = baseURL + '/api/license-plate-recognition';
export const listHistoryDetection = historyDetection + '/list';
export const historyAsc =
  historyDetection + '/list?SortBy=DETECTION_DATE&IsAscending=true&';
export const historySearchPlates = historyAsc + 'NumberPlates=';
export const historySearchSite = historyAsc + 'SiteIds=';

//Match Detection
export const matchDetection = baseURL + '/api/match';
export const listMatch = matchDetection + '/list';

//Site (Location)
export const site = baseURL + '/api/site';
export const siteData = site + '?id=';
export const listSite = site + '/list';

//Watchlist
export const watchlist = baseURL + '/api/watchlist';
export const watchlistDelete = watchlist + '?uid=';
export const watchlistUpdate = watchlist + '?id=';
export const listWatchlist = watchlist + '/list';

//Public IP
export const publicIP = baseURL + '/api/public-ip?siteId=';
