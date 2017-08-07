// modal component using axios
Vue.component('branchmodal', {
    template: '#modal-template',
    props: {
        details: [],
        lat: '',
        lng: '',
    },
    methods: {
        showModal: function () {
            var self = this;
            // get store json from server based on lat and lng returned from google
            axios.get('/GetBranches/' + self.lat + '/' + self.lng).then(function (response) {
                self.details = response.data; // array processed in for loop below
                self.showModal = true;
                var myLatLng = new google.maps.LatLng(self.lat, self.lng);
                var map_canvas = document.getElementById('map');
                var options = {
                    zoom: 10,
                    center: myLatLng,
                    mapTypeId: google.maps.MapTypeId.ROADMAP
                };
                map = new google.maps.Map(map_canvas, options);
                var center = map.getCenter();
                var i2 = 0;
                var infowindow = null;
                infowindow = new google.maps.InfoWindow({ content: "holding..." });
                // add the markers, event handlers, infowindows for each location
                for (i = 0; i < self.details.length; i++) {
                    i2 = i + 1;
                    marker = new google.maps.Marker({
                        position: new google.maps.LatLng(self.details[i].latitude, self.details[i].longitude),
                        map: map,
                        animation: google.maps.Animation.DROP,
                        icon: "../img/marker" + i2 + ".png",
                        title: "Store# " + self.details[i].id + " " + self.details[i].street + ", " + self.details[i].city + ", " + self.details[i].region,
                        html: "<div>Store# " + self.details[i].id + "<br/>" +
                        self.details[i].street + ", " + self.details[i].city + "<br/>" +
                        self.details[i].distance.toFixed(2) + " km</div>"
                    });

                    google.maps.event.addListener(marker, 'click', function () {
                        infowindow.setContent(this.html); // added .html to the marker
                        infowindow.close();
                        infowindow.open(map, this);
                    });
                }
                map.setCenter(center);
                google.maps.event.trigger(map, "resize");
            }, function (error) {
                console.log(error.statusText);
            })
        }
    },
    mounted: function () {
        this.showModal();
    },
});


// Vue instance using google maps geocoder
var app = new Vue({
    el: '#branches',
    methods: {
        loadAndShowModal: function () {
            var self = this;
            var geocoder = new google.maps.Geocoder(); // A service for converting between an address to LatLng

            geocoder.geocode({ "address": this.address }, function (results, status) {
                if (status == google.maps.GeocoderStatus.OK) { // only if google gives us the OK
                    self.lat = results[0].geometry.location.lat();
                    self.lng = results[0].geometry.location.lng();
                    self.showModal = true;
                }
            })
        },
    },
    data: {
        address: 'N5Y-5R6',
        lat: '',
        lng: '',
        showModal: false,
    }
});