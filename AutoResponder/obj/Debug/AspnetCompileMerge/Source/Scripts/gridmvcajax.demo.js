GridMvcAjax = {};
GridMvcAjax.demo = (function (my, $) {

    var constructorSpec = {
        updateGridAction: ''
    };

    my.init = function (options) {
        $(function () {
            constructorSpec = options;

            $.ajaxSetup({
                cache: false
            });
            pageGrids.carsGrid.ajaxify({
                getPagedData: constructorSpec.updateGridAction,
                getData: constructorSpec.updateGridAction
            });

        });
    };

    return my;
}(GridMvcAjax.demo || {}, jQuery));