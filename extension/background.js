<<<<<<< HEAD
console.log("TSEST");
=======
chrome.browserAction.onClicked.addListener(function(activeTab){
  var newURL = "http://turnthatsmileupsidedown.com/";
  chrome.tabs.create({ url: newURL });
});
>>>>>>> 416570cf0fc142467c1ae5514919a2c6c9e70f9d
