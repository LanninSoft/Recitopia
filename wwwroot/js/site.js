
//Will convert UTC time to local time on views.  Use the .posted-date class on each field you wish to convert
    $(document).ready(function () {
        $('.posted-date').each(function () {
            try {

                var text = $(this).html();

                var mydate = new Date(text);

                var serverDate = moment.utc(mydate).local().format('llll');

                $(this).html(serverDate);
            }
            catch (ex) {
                console.warn("Error converting date time", ex);
            }
        });
});
