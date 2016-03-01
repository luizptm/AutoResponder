
$(document).ready(function () {
    $("select").chosen();
});

$(document).ready(function () {
    GridMvc.lang.ptbr = {
        filterTypeLabel: "Tipo de filtro:",
        filterValueLabel: "Significado:",
        applyFilterButtonText: "Aplicar",
        filterSelectTypes: {
            Equals: "Igual",
            StartsWith: "Começa com",
            Contains: "Contém",
            EndsWith: "Termina com",
            GreaterThan: "Maior que",
            LessThan: "Menor que",
            GreaterThanOrEquals: "Maior que ou igual",
            LessThanOrEquals: "Menor que ou igual"
        },
        code: 'ptbr',
        boolTrueLabel: "Sim",
        boolFalseLabel: "Não",
        clearFilterLabel: "Limpar Filtro"
    };
    if (pageGrids.dataGrid != undefined) {
        pageGrids.dataGrid.lang = GridMvc.lang.ptbr;
    }
});

$(document).ready(function () {
    // Initialise Globalize to the current UI culture (you need to have served up the relevant culture file for this to work)
    //Globalize.culture("pt-BR"); //alreray done in 'jquery.validate.globalize.js'
});

function loadCKEditor(id, inline) {
    var instance = CKEDITOR.instances[id];
    if (instance) {
        CKEDITOR.remove(instance);
    }
    if (inline == true) {
        CKEDITOR.inline('Body', {
            uiColor: "#9AB8F3",
            //toolbar: [{ name: 'document', items: ['Source', '-', 'Maximize'] }],
            toolbar: [{ name: 'document', items: ['Maximize', 'Preview'] }],
            on:
            {
                instanceReady: function (ev) {
                    // Output paragraphs as <p>Text</p>.
                    this.dataProcessor.writer.setRules('p',
                        {
                            indent: false,
                            breakBeforeOpen: false,
                            breakAfterOpen: false,
                            breakBeforeClose: false,
                            breakAfterClose: false
                        });
                }
            }
        });
    }
    else {
        CKEDITOR.replace(id, {
            uiColor: "#9AB8F3",
            //toolbar: [{ name: 'document', items: ['Source', '-', 'Maximize'] }],
            toolbar: [{ name: 'document', items: ['Maximize', 'Preview'] }],
            on:
            {
                instanceReady: function (ev) {
                    // Output paragraphs as <p>Text</p>.
                    this.dataProcessor.writer.setRules('p',
                        {
                            indent: false,
                            breakBeforeOpen: false,
                            breakAfterOpen: false,
                            breakBeforeClose: false,
                            breakAfterClose: false
                        });
                }
            }
        });
    }
}

function exibirErro(data, textStatus, jqXHR) {
    var msg = "";
    if (jqXHR != undefined)
        msg = "<h3>" + jqXHR + "</h3>";
    var responseText = data.responseText.replace(/<br>/g, '');
    msg += "<div style='width: 550px; height:400px; overflow-y: scroll;'>" + responseText + "</div>";
    BootstrapDialog.show({
        type: BootstrapDialog.TYPE_DANGER, title: 'ERRO', cssClass: 'bs-example-modal-lg', message: msg, buttons: [{ label: 'OK', action: function (dialog) { dialog.close(); } }]
    });
}

function ExcluirRegistro() {
    var id = $("#Id").val();
    var token = $("input[name='__RequestVerificationToken']").val();
    var controller = document.location.pathname;
    var search = document.location.search;
    BootstrapDialog.show({
        type: BootstrapDialog.TYPE_DANGER, title: 'Confirma exclusão do registro?', message: "",
        buttons: [{
            label: 'Cancelar', cssClass: 'btn btn-default', action: function (dialog) {
                dialog.close();
            }
        },
        {
            label: 'OK', cssClass: 'btn btn-danger', action: function (dialog) {
                var $button = this; // 'this' here is a jQuery object that wrapping the <button> DOM element.
                $button.disable();
                $button.spin();
                $.ajax({
                    type: "POST",
                    url: controller + "/Delete/" + id,
                    data: { '__RequestVerificationToken': token },
                    success: function (data, textStatus, jqXHR) {
                        document.location.href = controller + search;
                    },
                    error: function (data, textStatus, jqXHR) {
                        exibirErro(data, textStatus, jqXHR);
                   }
                });
            }
        }]
    });
}