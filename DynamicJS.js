function EditNovedadModalReady() {
    if ($("#NovedadId").val() === "00000000-0000-0000-0000-000000000000") {
        $("#modalEditNovedad").modal('show');
    }
    else {
        $('#modalViewNovedad').modal('hide');

        $("#modalViewNovedad").on("hidden.bs.modal", function () {
            $("#modalEditNovedad").modal('show');
            //('body').addClass('modal-open');
        });
    }

    $("#modalEditNovedad").on("shown.bs.modal", function () {
        UpdateValidators('form-edit-novedad');
    });

    $("#drpTipoNovedadEdit").change(drpTipoNovedad_Change);

    try {
        $(".datepicker").datepicker({ dateFormat: 'dd/MM/yyyy' });
    } catch (e) {
    }

    $('[data-toggle="tooltip"]').tooltip()

    initAutoComplete();

    $("#btnUploadFoto").click(function () {
        btnUploadFoto_click();
    });

    $("#btnAgregarURL").click(function () {
        btnAgregarURL_click();
    });

    $("#fuFoto").change(function () {
        fuFoto_change();
    });

    drpTipoNovedad_Change();

    $("#btn-" + PersonaID).hide();
    if ($("#PersonaNovedadList").html().length < 50) {
        AddPersonaActual(PersonaID);
    }

    var camera = null;
    $('#takePhotoModal').on('shown.bs.modal', function () {
        if (camera) {
            camera.reset();
        }
        else {
            camera = $("#takePhotoContainer").Camera({
                width: 1280,
                height: 900,
                takeActionsContainer: $("#takePhotoActions"),
                editActionsContainer: $("#editPhotoActions"),
                takePhotoButton: $("#takePhotoButton"),
                retakePhotoButton: $("#retakePhotoButton"),
                acceptPhotoButton: $("#acceptPhotoButton"),
                cropperButtonRotateClockwise: $("#cropperRotateClockwise"),
                cropperButtonRotateCounterclockwise: $("#cropperRotateCounterClockwise"),
                cropperButtonZoomIn: $("#cropperZoomIn"),
                cropperButtonZoomOut: $("#cropperZoomOut"),
                cropperButtonZoomReset: $("#cropperZoomReset"),
                cropperButtonClear: $("#cropperClear"),
                cropperButtonReset: $("#cropperReset"),
                getData: function (data) {
                    uploadCameraPhoto(data);
                }
            });
        }
    });
}

function drpTipoNovedad_Change() {
    
    var tipoNovedadId = $("#drpTipoNovedadEdit").val();
    var tipoContenidoId = tipoNovedadContenido[tipoNovedadId];
    var auxFechaTitulo = tipoNovedadFechaTitulo[tipoNovedadId]; 
    var auxNumero = tipoNovedadInt[tipoNovedadId];
    var auxString = tipoNovedadString[tipoNovedadId];
    var auxBit = tipoNovedadBit[tipoNovedadId];
    var auxPersona = tipoNovedadPersona[tipoNovedadId];
    
    if (tipoContenidoId !== undefined) {
        $("#drpTipoContenidoEdit").val(tipoContenidoId);
        $("#drpTipoContenidoEdit").prop("disabled", true);
    }
    else {
        $("#form-amonestaciones").hide();
        //$("#drpTipoContenidoEdit").val(tipoContenidoNeutro);
        $("#drpTipoContenidoEdit").prop("disabled", false);
    }        
    if (tipoNovedadId === tipoNovedadAmonestaciones) {
        $("#form-amonestaciones").show();
    }
    else {
        $("#form-amonestaciones").hide();
    }

    if (auxFechaTitulo !== undefined) {
        $(".auxFecha").show();
        $("#lblAuxFecha").text(auxFechaTitulo);
        $("#HasFechaAux").val(true);
    }
    else {
        $(".auxFecha").hide();
        $("#HasFechaAux").val(false);
    }    

    if (auxPersona !== undefined) {
        $(".auxPersona").show();
        $("#lblAuxPersona").text(auxPersona);
        $("#HasAuxPersona").val(true);
    }
    else {
        $(".auxPersona").hide();
        $("#HasAuxPersona").val(false);
    } 

    if (auxNumero !== undefined) {
        $(".auxNumero").show();
        $("#lblAuxNumero").text(auxNumero);
        $("#HasAuxNumerico").val(true);
    }
    else {
        $(".auxNumero").hide();
        $("#HasAuxNumerico").val(false);
    } 
    if (auxString !== undefined) {
        $(".auxString").show();
        $("#lblAuxString").text(auxString);
        $("#HasAuxString").val(true);
    }
    else {
        $(".auxString").hide();
        $("#HasAuxString").val(false);
    } 
    if (auxBit !== undefined) {
        $(".auxBit").show();
        $("#lblAuxBit").text(auxBit);
        $("#HasAuxBit").val(true);
    }
    else {
        $(".auxBit").hide();
        $("#HasAuxBit").val(false);
    } 
    UpdateValidators('form-edit-novedad');
}

function novedadEditSuccess(data) {
    $("#DetailNovedad").html(data);
    $("#modalEditNovedad").modal("hide");
    if (CantidadNovedades[novedadListName] === 0) {
        loadNovedades();
    }
    else {
        loadNovedadesOnlyList();
    }
    if (novedadListName === 'datosMedicos') {
        novedadesLoaded = false;
        informacionAcademicaLoaded = false;
    }
    else if (novedadListName === 'informacionAcademica') {
        novedadesLoaded = false;
        datosMedicosLoaded = false;
    }
    else {
        datosMedicosLoaded = false;
        informacionAcademicaLoaded = false;
    }
}



function novedadBeforePost() {
    if ($("[name='TipoNovedadId']").val() === "") {
        $("#EditNovedadErrorMessage").html("Debe seleccionar el tipo de novedad ");
        $("#EditNovedadError").show();
        return false;
    }
    if ($("[name='TipoContenidoId']").val() === "") {
        $("#EditNovedadErrorMessage").html("Debe seleccionar el tipo de contenido");
        $("#EditNovedadError").show();
        return false;
    }


    $("#FooterNovedad").hide();
    $("#ProcessingNovedad").show();
}

function novedadEditError(a, b, c) {
    debugger;
    $("#EditNovedadErrorMessage").html(c);
    $("#EditNovedadError").show();
    $("#FooterNovedad").show();
    $("#ProcessingNovedad").hide();
}

function AddPersonaActual(personaid) {
    $.ajax({
        url: baseURL + "Novedad/AddPersonaNovedad?Personaid=" + personaid,
        cache: false,
        success: function (html) {
            $("#PersonaNovedadList").append(html);
            $("#btn-" + PersonaID).hide();
        },
        error: function (a, b, c) {
            $("#EditNovedadErrorMessage").html(c);
            $("#EditNovedadError").show();
        }
    });
}

function AddPersonaNovedad() {

    var valueSelected = $("#hdnPersonaId").val();
    var textSelected = $("#txtPersona").val();

    //var textSelected = $("#PersonaId option:selected").text();

    if (valueSelected === '') {
        $("#EditNovedadErrorMessage").html("Debe seleccionar una persona para agregar");
        $("#EditNovedadError").show();
        return;
    }
    if ($("#li-" + valueSelected).length > 0 || $("#hdn-id-" + valueSelected).length > 0) {
        $("#EditNovedadErrorMessage").html("La persona ya se encuentra agregada");
        $("#EditNovedadError").show();
        return;
    } else {
        $.ajax({
            url: baseURL + "Novedad/AddPersonaNovedad?Personaid=" + valueSelected,
            cache: false,
            success: function (html) {
                $("#PersonaNovedadList").append(html);
                $("#hdnPersonaRelacionId").val(null);
                $("#txtPersona").val(null);
            },
            error: function (a, b, c) {
                $("#EditNovedadErrorMessage").html(c);
                $("#EditNovedadError").show();
            }
        });
    }
}

function DeletePersonaNovedad(PersonaId) {
    $("#li-" + PersonaId).remove();
    $("#hdn-id-" + PersonaId).remove();
    $("#hdn-name-" + PersonaId).remove();
}


//AUTOCOMPLETE
function initAutoComplete() {
    $("#txtPersona").autocomplete({
        source: function (request, response) {
            CargarPersonaAutocomplete(request, response);
        },
        select: function (event, ui) {
            $("#hdnPersonaId").val(ui.item.id);
        },
        minLength: 3
    });
}

function CargarPersonaAutocomplete(request, response) {
    $.ajax({
        url: PersonaListAutocompleteURL,
        data: '{ "filtro": "' + request.term + '",' +
            '"PersonaId": "' + PersonaID + '"' + '}',
        dataType: "json",
        type: "POST",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            response($.map(data, function (item) {
                return {
                    id: item.PersonaId,
                    value: item.Nombre
                }
            }))
        },
        error: function (a, b, c) {
            $("#EditNovedadErrorMessage").html(c);
            $("#EditNovedadError").show();
        }
    });
}

function txtCliente_onKeyPress(e) {
    var code = e.keyCode || e.which
    if (code === 8) { //BACKSPACE
        $("#hdnPersonaRelacionId").val(null);
        $("#txtPersona").val(null);
    }
    else if (code === 13) { //ENTER
        addPersona_Click();
        e.preventDefault();
    }
}
//FIN AUTOCOMPLETE



//DELETE

function OpenModalDeleteNovedad(NovedadId) {
    $('#DeleteModal').modal('show');
    $('#DeleteModal').css('z-index', 9999);
    $('#NombreItem1').text('la novedad');
    $('#NombreItem2').text('la novedad');
    $("#btnBorrar").attr("onclick", "DeleteNovedad('" + NovedadId + "')");
}

function DeleteNovedad(NovedadId) {

    $.ajax({
        url: DeleteNovedadURL,
        data: '{ "NovedadId": "' + NovedadId + '"}',
        type: "POST",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            $('#DeleteModal').modal('hide');
            $("#SuccessDeleteModal").modal('show');
            $('#NombreItem3').text('la novedad');
            $('#modalEditNovedad').modal('hide');
            loadNovedadesOnlyList();
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $("#DeleteModalErrorMessage").html(errorThrown);
            $("#DeleteModalError").show();

        }
    });
}
//END DELETE
