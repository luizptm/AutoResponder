
var alert_div = "<div class='alert alert-warning'>Selecione uma lista</div>";

var msg_erro = "Erro no processamento.";

$(document).ready(function () {
    $("#SendingList").selectpicker();
	$("#usuarios").append(alert_div);
	$("#templates").append(alert_div);
	$("#envios").append(alert_div);
});

$(document).ready(function () {
    //DONT'T REMOVE THIS!!!
    //Bootstrap modal 'data-remote' correction:
	//source: http://stackoverflow.com/questions/12286332/twitter-bootstrap-remote-modal-shows-same-content-everytime
	$('body').on('hidden.bs.modal', '.modal', function () {
		$(this).removeData('bs.modal');
	});
});

$(document).ready(function () {
    // Initialise Globalize to the current UI culture (you need to have served up the relevant culture file for this to work)
    //Globalize.culture("pt-BR"); //alreray done in 'jquery.validate.globalize.js'
});

jQuery.validator.setDefaults({
	debug: true,
	success: "valid"
});

function loadCKEditor(id, inline) {
    var instance = CKEDITOR.instances[id];
    if (instance) {
        CKEDITOR.remove(instance);
    }
    if (inline == true) {
        CKEDITOR.inline(id, {
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
                    //$(".cke_source").attr("id", id);
                    //$(".cke_source").attr("name", id);
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
                    //$(".cke_source").attr("id", id);
                    //$(".cke_source").attr("name", id);
                }
            }
        });
    }
}

function RefreshListas() {
    var request = $.ajax({ url: "./MainSite/GetListas", type: "GET" });
    request.done(function (data) {
        $("#SendingList").html("");
        $("#SendingList").append(data);
        $('.selectpicker').selectpicker('render');
        $('.selectpicker').selectpicker('refresh');
    });
}

function ChangeLista() {
	var listId = $("#SendingList").val();
	if (listId != "") {
	    ListDetails();
	    Usuarios();
	    Templates();
	    Envios();
	}
	else {
	    $("#listDetails").html("");
	    $("#usuarios").html(""); $("#usuarios").append(alert_div);
	    $("#template").html(""); $("#template").append(alert_div);
	    $("#envios").html(""); $("#envios").append(alert_div);
	}
	Logs();
}

function ListDetails() {
    var id = $("#SendingList").val();
    var user_name = $("#user_name").val();
    user_name = user_name == undefined ? "" : user_name;
    if (id != "") {
        $.ajax({
            url: "/MainSite/ListDetails/", data: { 'SendingList': id },
            success: function (data, textStatus, jqXHR) {
                $("#listDetails").html(data);
            },
            error: function (data, textStatus, jqXHR) {
                exibirErro(data, textStatus, jqXHR);
            }
        });
    }
}

function UserDetails(link) {
    var href = link.attr("href");
    $.ajax({
        type: "GET", url: "/MainSite/UserDetails/" + id,
        success: function (data, textStatus, jqXHR) {
            var $content = data;
            BootstrapDialog.show({
                type: BootstrapDialog.TYPE_PRIMARY, title: "Detalhes do Usuário", message: $content,
                buttons: [{
                    label: 'OK', cssClass: 'btn btn-primary', action: function (dialog) {
                        dialog.close();
                    }
                }]
            });
        },
        error: function (data, textStatus, jqXHR) {
            exibirErro(data, textStatus, jqXHR);
        }
    });
    return false;
}


function Logs() {
	$.ajax({
	    type: "GET", url: "/MainSite/Logs",
	    success: function (data, textStatus, jqXHR) {
			$("#logs").html(data);
		},
		error: function (data, textStatus, jqXHR) {
			exibirErro(data, textStatus, jqXHR);
		}
	});
}

function Usuarios() {
	var listId = $("#SendingList").val();
    var user_name = $("#user_name").val();
    user_name = user_name == undefined ? "" : user_name;
    if (listId != "") {
		$.ajax({
			url: "/MainSite/Usuarios/", data: { 'SendingList': listId, 'user_name': user_name },
			success: function (data, textStatus, jqXHR) {
			    $("#usuarios").html(data);
			},
			error: function (data, textStatus, jqXHR) {
				exibirErro(data, textStatus, jqXHR);
			}
		});
	}
}

function Templates() {
	var listId = $("#SendingList").val();
	if (listId != "") {
        $.ajax({
        	url: "/MainSite/Templates/", data: { 'SendingList': listId },
            success: function (data, textStatus, jqXHR) {
                $("#templates").html(data);
            },
            error: function (data, textStatus, jqXHR) {
                exibirErro(data, textStatus, jqXHR);
            }
        });
    }
}

function Envios() {
	var listId = $("#SendingList").val();
    var user_name = $("#user_name").val();
    user_name = user_name == undefined ? "" : user_name;
    var Open = $("#cbOpen:checked").val();
    var Unsubscribe = $("#cbUnsubscribe:checked").val();
    var Bounce = $("#cbBounce:checked").val();
    var Click = $("#cbClick:checked").val();
    Open = Open == undefined ? "" : Open;
    Unsubscribe = Unsubscribe == undefined ? "" : Unsubscribe;
    Bounce = Bounce == undefined ? "" : Bounce;
    Click = Click == undefined ? "" : Click;
    if (listId != "") {
		$.ajax({
			url: "/MainSite/Envios/", data: { 'SendingList': listId, 'user_name': user_name, Open: Open, Unsubscribe: Unsubscribe, Bounce: Bounce, Click: Click },
			success: function (data, textStatus, jqXHR) {
			    $("#envios").html(data);
			},
			error: function (data, textStatus, jqXHR) {
				exibirErro(data, textStatus, jqXHR);
			}
		});
	}
}

function OnSuccess(data, action)
{
	if (data.Status == "0") {
		BootstrapDialog.show({
			type: BootstrapDialog.TYPE_WARNING, title: 'ATENÇÃO', message: data.Message, buttons: [{ label: 'OK', action: function (dialog) { dialog.close(); } }]
		});
	}
	else {
		exibirSucesso();
		var listId = $("#SendingList").val();
		if (listId != "") {
			Templates();
		}
		else {
			$("#template").html("");
			$("#template").append(alert_div);
		}
		if (action == "CreateSendingList") {
			RefreshListas();
		}
	}
}

function OnError(data, textStatus, jqXHR) {
	exibirErro(data, textStatus, jqXHR);
}

function exibirMensagem(data, modalId) {
	if (data.Status == "0") {
		BootstrapDialog.show({
			type: BootstrapDialog.TYPE_WARNING, title: 'ATENÇÃO', message: data.Message, buttons: [{ label: 'OK', action: function (dialog) { dialog.close(); } }]
		});
	}
	else {
		BootstrapDialog.show({
			type: BootstrapDialog.TYPE_SUCCESS, title: 'SUCESSO', message: 'Ação realizada com sucesso!', buttons: [{
				label: 'OK',
				action: function (dialog) {
					dialog.close();
					
				}
			}]
		});
	}
	if (modalId != undefined && data.Status == "1") {
        if (modalId == "CreateTemplate" || modalId == "EditarTemplate") {
	        Templates();
	    }
        else if (modalId == "CreateSendingList") {
	        RefreshListas();
	    }
	    else if (modalId == "CreateTag") {
	    	var request = $.ajax({ url: "./MainSite/GetTagsInDropDownList", type: "GET" });
	        request.done(function (data) {
	            $("#TagId").html("");
	            $("#TagId").append(data);
	            $('.selectpicker').selectpicker('render');
	            $('.selectpicker').selectpicker('refresh');
	        });
	    }
	}
	if (modalId != undefined && data.Status == "1") {
		$("#" + modalId).modal('hide');
	}
}

function exibirSucesso() {
    BootstrapDialog.show({
        type: BootstrapDialog.TYPE_SUCCESS, title: 'SUCESSO', message: 'Ação realizada com sucesso!', buttons: [{
            label: 'OK',
            action: function (dialog) {
                dialog.close();
            }
        }]
    });
}

function exibirErro(data, textStatus, jqXHR) {
    var msg = "";
    if (data.status == 500) {
        if (jqXHR != undefined)
            msg = "<h3>" + jqXHR + "</h3>";
        var responseText = data.responseText.replace(/<br>/g, '');
        msg += "<div style='width: 550px; height:400px; overflow-y: scroll;'>" + responseText + "</div>";
    } else if (data.status == 401) {
        msg = "Acesso negado";
    }
    else {
        msg = data.responseText;
    }
	BootstrapDialog.show({
		type: BootstrapDialog.TYPE_DANGER, title: 'ERRO', cssClass: 'bs-example-modal-lg', message: msg, buttons: [{ label: 'OK', action: function (dialog) { dialog.close(); } }]
	});
}

function ExecutarEnvio() {
    var button = $("#executarEnvio");
    button.attr("disabled", "disabled");
    button.addClass('icon-spin');
    var SendingListId = $("#SendingList").val();
    $.ajax({
        type: "GET", url: "/MainSite/ExecutarEnvio", data: { 'SendingListId': SendingListId },
        success: function (data, textStatus, jqXHR) {
            BootstrapDialog.show({
                type: BootstrapDialog.TYPE_SUCCESS, title: 'SUCESSO', message: data, buttons: [{
                    label: 'OK',
                    action: function (dialog) {
                        dialog.close();
                    }
                }]
            });
            if (data.search(/sucesso/g) > 0) {
            	Envios();
            	LerBounces();
            }
        },
        error: function (data, textStatus, jqXHR) {
            exibirErro(data, textStatus, jqXHR);
        }
    });
    
    Logs();
    button.removeAttr("disabled");
    button.removeClass('icon-spin');
}

function LerBounces() {
    var button = $("#lerBounces");
    button.attr("disabled", "disabled");
    button.addClass('icon-spin');
    var SendingListId = $("#SendingList").val();
    $.ajax({
        type: "GET", url: "/MainSite/LerBounces", data: {},
        success: function (data, textStatus, jqXHR) {
            if (data == "") {
                data = "Nenhum dado a exibir.";
            }
            if (data.indexOf("-ERR") >= 0) {
            	BootstrapDialog.show({
            		type: BootstrapDialog.TYPE_DANGER, title: 'ERRO', message: data, buttons: [{
            			label: 'OK',
            			action: function (dialog) {
            				dialog.close();
            			}
            		}]
            	});
            }
            else if (data.indexOf("+OK") >= 0) {
            	BootstrapDialog.show({
            		type: BootstrapDialog.TYPE_SUCCESS, title: 'SUCESSO', message: data + " bounces realizados/cadastrados.", buttons: [{
            			label: 'OK',
            			action: function (dialog) {
            				dialog.close();
            			}
            		}]
            	});
            }
            Envios();
        },
        error: function (data, textStatus, jqXHR) {
            exibirErro(data, textStatus, jqXHR);
        }
    });
    button.removeAttr("disabled");
    button.removeClass('icon-spin');
}

function ShowCreateSendingListPage() {
	$.ajax({
		type: "GET", url: "/MainSite/CreateSendingList/",
		success: function (data, textStatus, jqXHR) {
			$("#templates").html(data);
			$('#tabs li:eq(1) a').tab('show');
		},
		error: function (data, textStatus, jqXHR) {
			exibirErro(data, textStatus, jqXHR);
		}
	});
}

function ShowCreateTemplate(sequence) {
    var listId = $("#SendingList").val();
    $.ajax({
        type: "GET", url: "/MainSite/CreateTemplate/", data: { 'listId': listId },
        success: function (data, textStatus, jqXHR) {
            $("#templates").html(data);
        },
        error: function (data, textStatus, jqXHR) {
            exibirErro(data, textStatus, jqXHR);
        }
    });
}

function ShowEditTemplate(id) {
    $.ajax({
        type: "GET", url: "/MainSite/EditTemplate/", data: { 'id': id },
        success: function (data, textStatus, jqXHR) {
            $("#templates").html(data);
        },
        error: function (data, textStatus, jqXHR) {
            exibirErro(data, textStatus, jqXHR);
        }
    });
}

function DeleteTemplate(id, name) {
    BootstrapDialog.show({
        type: BootstrapDialog.TYPE_DANGER, title: 'Confirma?', message: "Template: " + name,
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
                    dataType: "json",
                    contentType: 'application/json',
                    url: "/MainSitePost/DeleteTemplate/" + id,
                    success: function (data, textStatus, jqXHR) {
                        exibirSucesso();
                        Templates();
                    },
                    error: function (data, textStatus, jqXHR) {
                        exibirErro(data, textStatus, jqXHR);
                    }
                });
                dialog.close();
            }
        }]
    });
}

function ShowAddUserToList(id) {
    var listId = $("#SendingList").val();
    $.ajax({
        type: "GET", url: "/MainSite/AddUserInList/" + listId,
        success: function (data, textStatus, jqXHR) {
            $("#usuarios").html(data);
        },
        error: function (data, textStatus, jqXHR) {
            exibirErro(data, textStatus, jqXHR);
        }
    });
}

function showContentOnModal(btn, title, id) { //NOT IN USE
    var url = "";
    if (id == "createTemplate") {
        url = "/MainSite/CreateTemplate/" + id;
    }
    if (id == "editarTemplate") {
        url = "/MainSite/EditTemplate/" + id;
    }
    $.ajax({
        type: "GET", url: url,
        success: function (data, textStatus, jqXHR) {
            var $content = data;
            BootstrapDialog.show({
                type: BootstrapDialog.TYPE_PRIMARY, title: title, message: $content,
                buttons: [{
                    label: 'Cancelar', cssClass: 'btn btn-default', action: function (dialog) {
                        dialog.close();
                    }
                },
                {
                    label: 'OK', cssClass: 'btn btn-primary', action: function (dialog) {
                        var $button = this; // 'this' here is a jQuery object that wrapping the <button> DOM element.
                        $button.disable();
                        $button.spin();
                        if (id == "createTemplate") {
                            CreateTemplate();
                        }
                        if (id == "editarTemplate") {
                            EditTemplate();
                        }
                        dialog.close();
                    }
                }]
            });
        },
        error: function (data, textStatus, jqXHR) {
            exibirErro(data, textStatus, jqXHR);
        }
    });
}

function renderSort(gridUrl, link, target) { //NOT IN USE!!!
    $.ajax({
        url: gridUrl,
        type: 'GET'
    }).done(function (response) {
        $("#" + target).html(response);
    });

    // remove grid sort arrows
    $(".grid-header-title").removeClass("sorted-asc");
    $(".grid-header-title").removeClass("sorted-desc");
    $(".grid-header-title").removeClass("sorted");

    var isAscending = gridUrl.indexOf("grid_dir=1") !== -1;
    var newSortClass = isAscending ? "sorted-desc" : "sorted-asc";
    newSortClass = "sorted " + newSortClass;

    // add sort classes
    link.parent(".grid-header-title").addClass(newSortClass);
    //remove span
    link.parent(".grid-header-title").children("span").remove();

    // add new grid sort arrow
    var span = $("<span/>").addClass("grid-sort-arrow");
    link.parent(".grid-header-title").append(span);

    // update link to sort in opposite direction
    if (isAscending) {
        gridUrl = gridUrl.replace("grid_dir=1", "grid_dir=0");
    } else {
        gridUrl = gridUrl.replace("grid_dir=0", "grid_dir=1");
    }
    link.attr("onclick", "javascript: renderSort('" + gridUrl + "', $(this), '" + target + "');");
}

function searchNameOrEmail(type, search) {
    search = search == undefined ? "" : search;
    if (search == "") {
        BootstrapDialog.show({
            type: BootstrapDialog.TYPE_DANGER, title: 'ERRO', message: "Campo não preenchido.",
            buttons: [{
                label: 'OK', action: function (dialog) {
                    dialog.close();
                }
            }]
        });
    }
    else {
        if (type == 'usuarios') {
            Usuarios();
        }
        else {
            Envios();
        }
    }
}

$('#btnSearchNameOrEmail').keypress(function (event) {
    if (event.keyCode == 13) {
        searchNameOrEmail();
    }
});