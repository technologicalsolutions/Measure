/*
* All Utilities Rene-Paramo-DEV
* Copyright (c) 2019 Rene Paramo
*
* This program is free software.
*/

function TextOnly(event) {
    //no se aceptan Espacion en blanco, acentos, Ñ/ñ    
    if ((event.charCode < 97 || event.charCode > 122) && (event.charCode < 65 || event.charCode > 90)) {
        event.returnValue = false;
    } else {
        event.returnValue = true;
    }
}

function TextOnlyUnder(event) {
    //esxcepcion : Acepta _ 
    //no se aceptan Espacion en blanco, acentos, Ñ/ñ    
    if ((event.charCode < 97 || event.charCode > 122) && (event.charCode < 65 || event.charCode > 90) && event.charCode != 95) {
        event.returnValue = false;
    } else {
        event.returnValue = true;
    }
}

function TextAndWhiteOnly(event) {
    //no se aceptan Espacion en blanco, acentos, Ñ/ñ    
    if ((event.charCode < 97 || event.charCode > 122) && (event.charCode < 65 || event.charCode > 90) && event.charCode != 32) {
        event.returnValue = false;
    } else {
        event.returnValue = true;
    }
}

function NumberOnly(event) {
    //no se aceptan Espacion en blanco, letras, acentos, Ñ/ñ
    if ((event.charCode < 48 || event.charCode > 57)) {
        event.returnValue = false;
    } else {
        event.returnValue = true;
    }
}

function NumberOnlyMax(event,max) {
    //no se aceptan Espacion en blanco, letras, acentos, Ñ/ñ
    if ((event.charCode < 48 || event.charCode > 57)) {        
        event.returnValue = false;
    } else {
        var newValue = parseInt(event.srcElement.value + String.fromCharCode(event.charCode));
        if (newValue > max) {
            event.returnValue = false;
        }
        event.returnValue = true;
    }
}

function DecimalOnly(e, obj) {
    //no se aceptan Espacion en blanco, letras, acentos, Ñ/ñ
    var key = window.Event ? e.which : e.keyCode;
    if (key === 46) {
        if (obj.value.includes(".") || obj.value === "") {
            event.returnValue = false;
        }
    }
    event.returnValue = (key >= 48 && key <= 57 || key === 46);
}

function deleteWhiteSpacesForPaste(e) {
    e.preventDefault();
    var pastedText = '';

    if (e.clipboardData && e.clipboardData.getData) {
        pastedText = e.clipboardData.getData('text/plain');
    }
    else if (window.clipboardData && window.clipboardData.getData) {// IE
        pastedText = window.clipboardData.getData('Text');
    }
    e.path[0].value = pastedText.trim().replace(/\s/g, '');
}

function SeparatorOnly(event) {
    //solo permite tipos de separador csv "," o ";" 0 "|"
    if (event.charCode == 44 || event.charCode == 59 || event.charCode == 124) {
        event.returnValue = true;
    } else {
        event.returnValue = false;
    }
}

function divLoading() {
    $("#divLoading").toggle();
}

function LoadDataTable(TableName, Select, Ordering, Data, PageLength) {
    var options = {
        "autoWidth": false,
        //dom: 'lBfrtip',
        //buttons: [
        //    { extend: 'copy', text: 'copiar'},
        //    { extend: 'csv', text: 'csv' },
        //],
        "responsive": true,
        "language": DataTableTranslate,
        "pageLength": PageLength,
    };
    if (Select) {
        options['columnDefs'] = [{
            orderable: false,
            className: 'select-checkbox',
            targets: 0
        }];
        options['select'] = {
            style: 'os',
            selector: 'td:first-child'
        };
    }
    if (Data != null) {
        options['columns'] = Data['Columns'];
        options['data'] = Data['DataSet'];
    }
    options["ordering"] = Ordering;

    $("#" + TableName).DataTable(options);
}

function seleccionarTodo(idTabla, idCheck) {
    var oldCheck = $("#" + idTabla + " tbody tr");
    var checkBox = document.getElementById(idCheck);
    if (checkBox.checked == true) {
        $.each(oldCheck, function (val, item) {
            $(item).addClass("selected")
            item.cells[0].firstElementChild.checked = true;
            item.cells[0].lastElementChild.value = true;
        });
    } else {
        $.each(oldCheck, function (val, item) {
            if ($(item)[0].className.includes("selected")) {
                $(item).removeClass("selected");
                item.cells[0].firstElementChild.checked = false;
                item.cells[0].lastElementChild.value = false;
            }
        })
    }
}

function DeseleccionarFila(Obj,item) {
    if (Obj.checked) {
        $(Obj.parentElement.parentElement).addClass("selected")
        if (item == undefined) {
            Obj.parentElement.lastElementChild.value = true;
        }        
    } else {
        if (Obj.parentElement.parentElement.className.includes("selected")) {
            $(Obj.parentElement.parentElement).removeClass("selected");
            if (item == undefined) {
                Obj.parentElement.lastElementChild.value = false;
            }            
        }        
    }
}

function ContarSeleccionados(idTabla) {
    var Filas = $("#" + idTabla + " tbody tr");
    var count = 0;
    $.each(Filas, function (pos, item) {
        if (item.cells[0].firstElementChild.checked) {
            count++;            
        }        
    });

    var Result = '';
    if (count == 0) {
        Result = 'Debe de seleccionar minimo una fila';
    }    
    return Result;
}

function ShowModal(name) {
    $("#"+name).modal("show");
}

function CloseModal(name) {
    $("#" + name).modal("hide");
}