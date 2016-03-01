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
            if (pageGrids.dataGrid_usuarios != undefined) {
                pageGrids.dataGrid_usuarios.ajaxify({
                    getPagedData: constructorSpec.updateGridAction,
                    getData: constructorSpec.updateGridAction
                });
            }
            if (pageGrids.dataGrid_envios != undefined) {
                pageGrids.dataGrid_envios.ajaxify({
                    getPagedData: constructorSpec.updateGridAction,
                    getData: constructorSpec.updateGridAction
                });
            }
        });
    };

    return my;
}(GridMvcAjax.demo || {}, jQuery));