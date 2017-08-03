var link = '/GetUsersOrders';
var detailslink = '/GetOrderDetails/';

// Register modal Component
Vue.component('modal', {
    template: '#modal-template',
    props: {
        item: {},
        modalItem: {},
        details: []
    },
});


new Vue({
    el: '#orders',
    methods: {
        getOrders: function () {
            axios.get(link)
                .then((response) => {
                    this.orders = response.data;
                })
                .catch((error) => {
                    this.error = error.response.data
                });
        },
        loadAndShowModal: function () {
            axios.get(detailslink + this.modalItem.id)
                .then((response) => {
                    this.details = response.data;
                    this.showModal = true;
                })
                .catch((error) => {
                    console.log(error.statusText);
                })
        },
    },
    mounted: function () {
        this.getOrders();
    },
    data: {
        orders: [],
        error: null,
        showModal: false,
        modalItem: {},
        details: []
    }
});



function formatDate(date) {
    var d;
    if (date === undefined) {
        d = new Date(); //no date coming from server
    }
    else {
        var d = new Date(Date.parse(date)); // date from server
    }
    var _day = d.getDate();
    var _month = d.getMonth() + 1;
    var _year = d.getFullYear();
    var _hour = d.getHours();
    var _min = d.getMinutes();
    if (_min < 10) { _min = "0" + _min; }
    return _year + "-" + _month + "-" + _day + " " + _hour + ":" + _min;
}

function cur(num) {
    num = num.toString().replace(/\$|\,/g, '');
    if (isNaN(num))
        num = "0";
    sign = (num == (num = Math.abs(num)));
    num = Math.floor(num * 100 + 0.50000000001);
    cents = num % 100;
    num = Math.floor(num / 100).toString();
    if (cents < 10)
        cents = "0" + cents;
    for (var i = 0; i < Math.floor((num.length - (1 + i)) / 3); i++)
        num = num.substring(0, num.length - (4 * i + 3)) + ',' +
            num.substring(num.length - (4 * i + 3));
    return (((sign) ? '' : '-') + '$' + num + '.' + cents);
} //cur

