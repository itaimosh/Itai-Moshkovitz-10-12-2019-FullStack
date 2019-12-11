const WEATHERICONURL = "https://developer.accuweather.com/sites/default/files/";
const WeatherAPI = "http://localhost:62667/api/weather/";
const FavoritesAPI = "http://localhost:62667/api/favorites/";


//Handle click on cities list 
function resultshandler(li) {

    let cityKey = li[0].id;
    let cityName = li[0].innerText;
    let url = `${WeatherAPI}GetCurrentWeather/${cityKey}`
    

    //Try to load city data from SQL DB. if Not found -> then load it from Accu Wather API
    axios.get(url)
        .then(function (response) {

            let resData = response.data;
            ShowSelectedWeather(cityKey, cityName, resData.weatherText, resData.temperature, resData.icon);
        })
        .catch(function (err) {
            console.log(err)
        }).finally(function () {  });
        
}


//Show weather of selected city on panel
function ShowSelectedWeather(cityKey, localizedName, weatherText, temperature, weatherIcon) {

    let selectedCityArray = new Array();

    selectedCityArray.push({ "key": cityKey, "localizedName": localizedName, "temperature": temperature, "weatherText": weatherText, "iconSrc": getIconSrc(weatherIcon) });
    let markup = createMarkup(selectedCityArray, isFavorites());
    document.getElementById("selectedCity").innerHTML = markup;

    $('ul#lstCities').find('li.active').removeClass('active');
    $('ul#lstCities').find(`li#${cityKey}`).addClass('active');
}

//Generate link to icon library from AccuWeather website
function getIconSrc(iconId) {
    return WEATHERICONURL + ("0" + iconId).slice(-2) +"-s.png";
}



function isFavorites() {
    var path = window.location.pathname;
    var page = path.split("/").pop();
    if (page == "favorites.html") {
        return true;
    }
    return false;
}

//Search for city on AccuWeather API. I want sure from the instructions if I should implemnt it as autocompleate or by click event of the search button.
function search() {
    let query = document.getElementById("txtSearch").value;
    
    let url = `${WeatherAPI}/search/${query}`;
    axios.get(url)
        .then(function (response) {
            var list = document.getElementById("lstCities");
            list.innerHTML = "";
            for (const item of response.data) {
                var li = document.createElement('li');
                li.id = item.key;
                li.setAttribute("class", "list-group-item");               
                li.appendChild(document.createTextNode(item.localizedName));
                list.appendChild(li);
            }

            $('ul#lstCities li').click(function () {
                resultshandler($(this));
            });

        })
        .catch(function (err) {
            console.log(err);
        }).finally(function () {
            
        });
    
}

//Save city to favorites table in DB
function addToFavorites(cityKey) {
   
    let localizedName = document.getElementById("city").innerText;
    const data = {
        Key: cityKey,
        LocalizedName: localizedName
    }

    axios.post(FavoritesAPI, data)
            .then(function (response) {
                toastr.info('City added to faivorites');
                location.href = `favorites.html?key=${cityKey}`;
            })
            .catch(function (err) {
                console.log(err);
            });

    
}

//Remove favorite city from favorites table in DB
function removeFromFavorites(cityKey) {
    axios.delete(FavoritesAPI + "/" + cityKey)
        .then(function (response) {
            document.getElementById(cityKey).outerHTML = "";
            $(`li#${cityKey}`).remove();
        })
        .catch(function (err) {
            console.log(err);
        });
        
}


//Load favorites data. try to load city weather from DB if exists. if not exists -> load it from AccuWeather API
function loadFavorites() {
    let favoritesArray = new Array();

    let url = new URL(window.location.href);
    let urlKey = url.searchParams.get("key");

    //load all favorites
    const promise1 = axios.get(FavoritesAPI);
    //load cities data
    const promise2 = axios.get(WeatherAPI);
    let promises = [];

    Promise.all([promise1, promise2]).then(function (values) {

        let favorites = values[0].data;
        let cities = values[1].data;

        favorites.forEach(function (favorite) {
            //Try to find city weather for every favorite city
            let city = cities.find(o => o.key == favorite.key);
           
            favoritesArray.push({ "key": city.key, "localizedName": favorite.localizedName, "temperature": city.temperature, "weatherText": city.weatherText, "iconSrc": getIconSrc(city.icon) });
           
        });
        

        var list = document.getElementById("lstCities");
        list.innerHTML = "";

        favoritesArray.forEach(function (item) {

            var li = document.createElement('li');
            li.id = item.key;
            if (item.key == urlKey) {
                li.setAttribute("class", "list-group-item active");
            }
            else {
                li.setAttribute("class", "list-group-item");
            }
            li.appendChild(document.createTextNode(item.localizedName));
            list.appendChild(li);

        });
            
        $('ul#lstCities li').click(function () {              
            resultshandler($(this));
        });

        $('ul#lstCities').find('li.active').click();
        
    });

   
    
}

//Generate City Wather markup
function createMarkup(items, isFavorites) {
    const markup =
        `<div id="weatherItems" class="top-buffer">
        ${items.map(item =>
            `<div id="${item.key}" class="row parent-item">
                    <div class="col-sm-8 col-md-10 col-lg-11">
                          <div class="row">
                                <div class="col-sm-9 col-lg-9 text-left">
                                    <h2 id="city">${item.localizedName}</h2>
                                    <div class="row">
                                          <div class="col-sm-3 col-lg-2">
                                            <h3>${item.temperature}<span>&#8451;</span> </h3>
                                          </div>
                                          <div class="col-sm-7 col-lg-4">
                                            <h3>${item.weatherText}</h3>
                                          </div>
                                          <div class="col-sm-2 col-lg-1" style="padding-top:10px">
                                             <img id="weatherIcon" src="${item.iconSrc}">
                                          </div>
                                    </div>
                                </div>
                                <div class="col-sm-3 col-lg-3" style="padding-top:20px">
                                    ${!isFavorites ? `<button id="btnAddtoFavorites" type="button" class="view-button btn btn-primary" onclick="addToFavorites(${item.key})" ><span class="glyphicon glyphicon-heart"> </span>&nbsp;Add to Favorites</button>` :
                                    `<button id="btnRemoveFromFavorites" type="button" class="view-button btn btn-primary" onclick="removeFromFavorites(${item.key})"><span class="glyphicon glyphicon-remove"> </span>&nbsp;Remove from Favorites</button>`}
                                </div>
                          </div>
                    </div>
                    </div>`).join('').trim()}
                </div>`

    return markup;



}



