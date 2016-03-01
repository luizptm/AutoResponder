
function CreateSendingListPage() {
    var token = $("input[name='__RequestVerificationToken']").val();
    var Name = $("#Name").val();
    var Description = $("#Description").val();
    var Tags = $("#Tags").val();
    
    $.ajax({
        type: "POST",
        url: "MainSitePost/CreateSendingList/",
        data: {
            "__RequestVerificationToken": token,
            "Name": Name,
            "Description": Description,
            "Tags": Tags
        },
        success: function (data, textStatus, jqXHR) {
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
        },
        error: function (data, textStatus, jqXHR) {
            exibirErro(data, textStatus, jqXHR);
        }
    });
} /* OLD POST ACTIONS */

function CreateTemplate() { /* OLD POST ACTIONS */
    var token = $("input[name='__RequestVerificationToken']").val();
    var SendingListId = $("#SendingListId").val();
    var Sequence = $("#Sequence").val();
    var Interval = $("#Interval").val();
    var Sender = $("#Sender").val();
    var Subject = $("#Subject").val();
    var Body = CKEDITOR.instances.Body.getData();

    $.ajax({
        type: "POST",
        url: "MainSitePost/CreateTemplate/",
        data: {
                "__RequestVerificationToken": token,
                "SendingListId" : SendingListId,
                "Sequence" : Sequence,
                "Interval" : Interval,
                "Sender" : Sender,
                "Subject" : Subject,
                "Body" : Body
        },
        success: function (data, textStatus, jqXHR) {
            if (data.Status == "0") {
                BootstrapDialog.show({
                    type: BootstrapDialog.TYPE_WARNING, title: 'ATENÇÃO', message: data.Message, buttons: [{ label: 'OK', action: function (dialog) { dialog.close(); } }]
                });
            }
            else {
                exibirSucesso();
                Templates();
            }
        },
        error: function (data, textStatus, jqXHR) {
            exibirErro(data, textStatus, jqXHR);
        }
    });
}

function EditTemplate() {/* OLD POST ACTIONS */
    var token = $("input[name='__RequestVerificationToken']").val();
    var Id = $("#Id").val();
    var SendingListId = $("#SendingListId").val();
    var Sequence = $("#Sequence").val();
    var Interval = $("#Interval").val();
    var Sender = $("#Sender").val();
    var Subject = $("#Subject").val();
    var Body = CKEDITOR.instances.Body.getData();
    var CREATION_DATE = $("#CREATION_DATE").val();

    $.ajax({
        type: "POST",
        url: "MainSitePost/EditTemplate/",
        data: {
            "__RequestVerificationToken": token,
            "Id": Id,
            "SendingListId": SendingListId,
            "Sequence": Sequence,
            "Interval": Interval,
            "Sender": Sender,
            "Subject": Subject,
            "Body": Body,
            "CREATION_DATE": CREATION_DATE
        },
        success: function (data, textStatus, jqXHR) {
            if (data.Status == "0") {
                BootstrapDialog.show({
                    type: BootstrapDialog.TYPE_WARNING, title: 'ATENÇÃO', message: data.Message, buttons: [{ label: 'OK', action: function (dialog) { dialog.close(); } }]
                });
            }
            else {
                //$("#EditTemplate").attr("class", "modal fade");
                //$(".modal-backdrop").remove();
                exibirSucesso();
                Templates();
            }
        },
        error: function (data, textStatus, jqXHR) {
            exibirErro(data, textStatus, jqXHR);
        }
    });
}

function AddUserToList() { /* OLD POST ACTIONS */
	var token = $("input[name='__RequestVerificationToken']").val();
	var UserId = $("#UserId").val();
	var SendingListId = $("#SendingListId").val();
	var EntryDate = $("#EntryDate").val();
	var Optin = $("#Optin").val();

	$.ajax({
		type: "POST",
		url: "MainSitePost/AddUserInList/",
		data: {
			"__RequestVerificationToken": token,
			"UserId": UserId,
			"SendingListId": SendingListId,
			"EntryDate": EntryDate,
			"Optin": Optin
		},
		success: function (data, textStatus, jqXHR) {
			if (data.Status == "0") {
				BootstrapDialog.show({
					type: BootstrapDialog.TYPE_WARNING, title: 'ATENÇÃO', message: data.Message, buttons: [{ label: 'OK', action: function (dialog) { dialog.close(); } }]
				});
			}
			else {
				exibirSucesso();
				Usuarios();
			}
		},
		error: function (data, textStatus, jqXHR) {
			exibirErro(data, textStatus, jqXHR);
		}
	});
}

function DetectSPAM(text) {
	if (text != "") {
		
		$.ajax({
			type: "POST",
			url: "/MainSitePost/DetectSPAM",
			data: {
				"text": text
			},
			success: function (data, textStatus, jqXHR) {
				process_spam_data(data);
			},
			error: function (data, textStatus, jqXHR) {
				exibirErro(data, textStatus, jqXHR);
			}
		});
	} else {
		BootstrapDialog.show({
			type: BootstrapDialog.TYPE_WARNING,
			title: 'ATENÇÃO',
			message: "O conteúdo está vazio!",
			buttons: [{ label: 'OK', action: function (dialog) { dialog.close(); } }]
		});
	}
}

function process_spam_data(data) {
	var title = "";
	var message = "";
	var type = null;
	if (data.IsSpam == true) {
		title = 'ATENÇÃO';
		message = "Conteúdo é SPAM!";
		type = BootstrapDialog.TYPE_WARNING;
	} else if (data.IsSpam == false) {
		title = 'APROVADO';
		message = "Conteúdo não é SPAM!";
		type = BootstrapDialog.TYPE_SUCCESS;
	} else if (data.Result == null) {
		title = 'SEM RESPOSTA';
		message = "Erro no processamento";
		type = BootstrapDialog.TYPE_DANGER;
	} else if (data.Result == "missing required parameters email or options") {
		title = 'MISSING PARAMETERS ERROR';
		message = "Parâmetros obrigatórios estão fatando";
		type = BootstrapDialog.TYPE_DANGER;
	} else {
		title = 'ERRO INESPERADO';
		message = data.Result;
		type = BootstrapDialog.TYPE_DANGER;
	}
	BootstrapDialog.show({
		type: type,
		title: title,
		message: message,
		buttons: [{ label: 'OK', action: function (dialog) { dialog.close(); } }]
	});
}