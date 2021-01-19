
$(function () {


    $('#dateShipment').datepicker({
        dateFormat: 'mm/dd/yy',
        changeMonth: true,
        changeYear: true,
        yearRange: "-15:+15"
    });
    $('#custName').val("")
    $('#vehCode').val("")
    $('#drvName').val("")
    $('#dateShipment').val("")
    $('#carrName').val("")
    //////////////////////////////////////////////////////Custom Validation/////////////////////////////////////////////////////////////

    $('#formAll').validate({
        errorClass: "text-danger",
        debug: true,
        rules: {
            ShipmentDate: {
                required: true,
            },
            AccessCardID: {
                required: true,
            },
            BayID: {
                required: true,
            }
        },
        messages: {
            ShipmentDate: {
                required: "Shipment Date is required."
            },
            AccessCardID: {
                required: "Access Card is required."
            },
            BayID: {
                required: "Bay is required."
            }
        }
    });

    ///////////////////////////////////////////////////////submit ob click event/////////////////////////////////////////////////////////////////
    $(document).on('click', 'form button[type=submit]', function (e) {
        debugger;
        var isValid = $('#formAll').valid();
        if (!isValid) {
            e.preventDefault(); //prevent the default action
            return;
        }
        else if ($('#compTable tr').length == 0) {
            $("#errorMessagesOnSave").text("No Compartment found");
            e.preventDefault();
            return;
        }
        else if (e.target.id == "close") {
            for (var i = 0; i < $('#compTable tr').length; i++) {
                var plannedQty = parseInt($('#compPlannedQty' + i).val())
                var actualQty = parseInt($('#compActualQty' + i).val())
                if (plannedQty != 0 && isNaN(actualQty)) {
                    $("#errorMessagesOnSave").text("Actual Quantity cannot be null, Check manual entry checkbox and correct the values");
                    e.preventDefault();
                    return;
                }
            }
            $("#shipmentStatusId").val(5);
        }
        else if ($('#compTable tr').length > 0) {
            for (var i = 0; i < $('#compTable tr').length; i++) {
                $("#compPlannedQty" + i).rules("add", {
                    required: true,
                    digits: true,
                    messages: {
                        required: "Only digits allowed"
                    }
                });
            }
            var newValid = $('#formAll').valid();
            if (!newValid) {
                e.preventDefault(); //prevent the default action
                return;
            }
            else {
                var sumOfPlannedQty = 0;
                for (var i = 0; i < $('#compTable tr').length; i++) {
                    sumOfPlannedQty = sumOfPlannedQty + parseInt($('#compPlannedQty' + i).val());
                    if (parseInt($('#compPlannedQty' + i).val()) > parseInt($('#compCapacity' + i).val())) {
                        $("#errorMessagesOnSave").text("Planned Quantity must not be greater than the compartment capacity");
                        e.preventDefault();
                        return;
                    }

                }
                if (sumOfPlannedQty != parseInt($('#odrQty').val())) {
                    $("#errorMessagesOnSave").text("Sum of Planned Quantity must be equal to the ordered Quantity");
                    e.preventDefault();
                    return;
                }

                $("#errorMessagesOnSave").text(" ");
            }

        }
    });

    $("#formAll").submit(function (e) {
        if ($("#errorMessagesOnSave").text() != " ") {
            e.preventDefault();
            return;
        }
        else {
            e.preventDefault(); //prevent default form submit

            $.ajax({
                type: "post",
                url: '@Url.Action("cardAssigned", "Shipments")',
                data: { cardId: $("#cardNo").val() },
                datatype: "json",
                traditional: true
            }).done(function myfunction(data) {
                try {
                    if (data == false) {
                        if ($("#manualEntry").is(":checked")) {
                            $("#shipmentStatusId").val(4);
                        }
                        $("#errorMessagesOnSave").text(" ");
                        $("#formAll").unbind('submit').submit();

                    }
                    else {
                        if (data == $("#shipmentId").val()) {
                            if ($("#manualEntry").is(":checked")) {
                                $("#shipmentStatusId").val(4);
                            }
                            $("#errorMessagesOnSave").text(" ");
                            $("#formAll").unbind('submit').submit();
                        }
                        else {
                            $("#errorMessagesOnSave").text("Card Already in use");
                        }

                    }
                }
                catch (err) {
                    alert(err.message);
                }

            })
        }
    });





    ////////////////////////////////////form submit//////////////////////////////////////////////////////
    $('#bayName').change(function () {
        debugger;
        $.ajax({
            type: "post",
            url: '/Shipments/SelectCard',
            data: { bayId: $("#bayName").val() },
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            traditional: true
        }).done(function myfunction(data) {
            $('#cardNo').empty();
            for (var i = 0; i < data.length; i++) {
                $('#cardNo').append('<option value="' + data[i].Value + '">' + data[i].Text + '</option>');
            }
        })
    })

    //////////////////////////////////////////////////card number on change end/////////////////////////////////////////////////


});
function edit(id, isManual, accessCard, modifiedDate, DeletedFlag, createdDate, shipStatusId, orderId, bayId, bayName, customerName, vehicleCode, driverName, driverCnic, carrierName, shipmentDate, productId, vehicleId, odrQty, odrCode) {


    $("#errorMessagesOnSave").text(" ");

    //////////////////////////////////////////////////////////////////////bay///////////////////////////////////////////////////////////////////////////////////
    $.ajax({
        type: "post",
        url: '/Shipments/SelectBay',

        data: { prdId: productId },
        datatype: "json",
        traditional: true
    }).done(function myfunction(data) {
        $('#bayName').empty();
        for (var i = 0; i < data.length; i++) {
            $('#bayName').append('<option value="' + data[i].Value + '">' + data[i].Text + '</option>');
        }
        if (bayId != 0) {
            $("#bayName").val(bayId);
        }
        else {
            $("#bayName").val(data[0].Value);
            bayId = data[0].Value;
        }
        //////////////////////////////////////////////////////card///////////////////////////////////////////////////////////
        $.ajax({
            type: "post",
            url: '/Shipments/SelectCard',
            data: {
                bayId: bayId
            },
            datatype: "json",
            traditional: true
        }).done(function myfunction(data) {
            $('#cardNo').empty();
            for (var i = 0; i < data.length; i++) {
                $('#cardNo').append('<option value="' + data[i].Value + '">' + data[i].Text + '</option>');
            }
            $("#cardNo").val(accessCard);
        })
    })
    ////////////////////////////////////////////////////////////////////////end bay//////////////////////////////////////////////////////////////////////////////////////


    $("#shipmentId").val(id)

    $("#shipmentStatusId").val(shipStatusId)
    $("#productId").val(productId)
    $("#odrQty").val(odrQty)
    $("#odrId").val(orderId)
    $("#custName").val(customerName)
    $("#vehCode").val(vehicleCode)
    $("#drvName").val(driverName)
    $("#drvCNIC").val(driverCnic)
    $("#carrName").val(carrierName)

    $("#dateShipment").val(shipmentDate)

    if (isManual == "onclick") {
        $("#manualEntry").prop('checked', true);
    }
    else {
        $("#manualEntry").prop('checked', false);
    }





    var prd;
    if (productId == 1) {
        prd = "LPG"
    }
    else {
        prd = "Condensate"
    }
    $.ajax({
        type: "post",
        url: '/Shipments/compartmentPlanning',
        data: {
            vehicleId: vehicleId,
            id: id
        },
        datatype: "json",
        traditional: true
    }).done(function myfunction(data) {
        if (data == false) {
            $('#compTable').empty();
        }
        else {
            $('#compTable').empty();
            if (data[0].isCreatedLI == true) {
                for (var i = 0; i < data.length; i++) {
                    $('#compTable').append('<tr class="item"><input type="hidden" name="compId' + i + '" id="compId' + i + '" value= "' + data[i].CompIdLI + '" readonly /><td><input type="text" name="compCode' + i + '" id="compCode' + i + '" value= "' + data[i].CompCodeLI + '" readonly /></td><td><input type="text" name="compOdrCode' + i + '" id="compOdrCode' + i + '" value= "' + odrCode + '" readonly /></td><td><input type="text" name="compPrd' + i + '" id="compPrd' + i + '" value= "' + prd + '" readonly /></td><td><input type="text" name="compOdrQty' + i + '" id="compOdrQty' + i + '" value= "' + data[i].OrderedQtyLI + '" readonly /></td><td><input type="text" name="compCapacity' + i + '" id="compCapacity' + i + '" value= "' + data[i].CapacityLI + '" readonly /></td><td><input type="number" name="compPlannedQty' + i + '" id="compPlannedQty' + i + '" value= "' + data[i].PlannedQtyLI + '" /></td><td><input type="text" name="compActualQty' + i + '" id="compActualQty' + i + '" value= "' + data[i].ActualQtyLI + '" readonly /></td></tr>');
                }
            }
            else {
                for (var i = 0; i < data.length; i++) {
                    $('#compTable').append('<tr class="item"><td><input type="text" name="compCode' + i + '" id="compCode' + i + '" value= "' + data[i].Text + '" readonly /></td><td><input type="text" name="compOdrCode' + i + '" id="compOdrCode' + i + '" value= "' + odrCode + '" readonly /></td><td><input type="text" name="compPrd' + i + '" id="compPrd' + i + '" value= "' + prd + '" readonly /></td><td><input type="text" name="compOdrQty' + i + '" id="compOdrQty' + i + '" value= "' + odrQty + '" readonly /></td><td><input type="text" name="compCapacity' + i + '" id="compCapacity' + i + '" value= "' + data[i].Value + '" readonly /></td><td><input type="number" name="compPlannedQty' + i + '" id="compPlannedQty' + i + '" /></td><td><input type="text" name="compActualQty' + i + '" id="compActualQty' + i + '" value= "' + 0 + '" readonly /></td></tr>');
                }
            }
        }

    })

    /////////////////////////////////finally unlock and lock buttons////////////////////////////////////////////
    switch (shipStatusId) {
        case "1":
            $("#save").prop("disabled", false);
            $("#loadSlip").prop("disabled", true);
            $("#close").prop("disabled", true);
            $("#bol").prop("disabled", true);
            break;
        case "2":
            $("#save").prop("disabled", false);
            $("#loadSlip").prop("disabled", false);
            $("#close").prop("disabled", true);
            $("#bol").prop("disabled", true);

            break;
        case "3":
            $("#save").prop("disabled", false);
            $("#loadSlip").prop("disabled", false);
            $("#close").prop("disabled", false);
            $("#bol").prop("disabled", true);
            break;
        case "4":
            $("#save").prop("disabled", true);
            $("#loadSlip").prop("disabled", true);
            $("#close").prop("disabled", true);
            $("#bol").prop("disabled", true);
            break;
        case "5":
            $("#save").prop("disabled", true);
            $("#loadSlip").prop("disabled", true);
            $("#close").prop("disabled", true);
            $("#bol").prop("disabled", false);
            break;

    }
    if (isManual == "onclick") {
        $("#save").prop("disabled", true);
        $("#loadSlip").prop("disabled", true);
        $("#close").prop("disabled", true);
        $("#bol").prop("disabled", true);
    }


    //////////////////////////////////////////////print slips //////////////////////////////////////////////////////////////////////////////////////////////////



    var date = new Date();
    date = date.toLocaleString('en-US');

    var shipdate = id + "(" + date + ")";

    $("#lbl_Ship").text(shipdate);
    $("#lbl_Cust").text(customerName);
    $("#lbl_Drvr").text(driverName);
    $("#lbl_RFID").text(accessCard);
    $("#lbl_Vno").text(vehicleCode);
    $("#lbl_Ocode").text(odrCode);
    $("#lbl_Prod").text(prd);
    $("#lbl_Qty").text(odrQty);
    $("#lbl_Bay").text(bayId);
    //////////////////////////////////////
    $("#lbl_Ship1").text(shipmentDate);
    $("#lbl_Cust1").text(customerName);
    $("#lbl_Drvr1").text(driverName);
    ////////////////////////////////////

    ////////////////////////////////////////////////BoL Slip////////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////
    $("#lbl_Shipbol").text(id);
    $("#lbl_Custbol").text(customerName);
    $("#lbl_Vehbol").text(vehicleCode);
    ////////////////////////////////////
    //////////////////////////////////////
    $("#lbl_DDatebol").text(shipmentDate);
    $("#lbl_EntTmebol").text(createdDate);
    $("#lbl_ExtTimebol").text(modifiedDate);
    ////////////////////////////////////
    $("#lbl_OdrCdebol").text(odrCode);
    $("#lbl_prodbol").text(prd);
    $("#lbl_Qtybol").text(odrQty);
    ////////////////////////////////////






}
function BoLPressed() {

    for (k = 1; k < 9; k++) {
        $('#lbl_Compidbol' + k).text(" ");
        $('#lbl_CActQtybol' + k).text(" ");
        $('#lbl_CPlnQtybol' + k).text(" ");
    }
    var countTr = $('#compTable tr').length;
    var j = 0;
    var sumOfPlannedQty = 0;
    var sumOfActualQty = 0;
    for (var i = 0; i < countTr; i++) {


        if ($.isNumeric(parseInt($('#compPlannedQty' + i).val()))) {
            sumOfActualQty = sumOfActualQty + parseInt($('#compActualQty' + i).val());
            sumOfPlannedQty = sumOfPlannedQty + parseInt($('#compPlannedQty' + i).val());
            j++;
            $('#lbl_Compidbol' + j).text(parseInt($('#compCode' + i).val()))
            $('#lbl_CActQtybol' + j).text(parseInt($('#compActualQty' + i).val()))

            //$('#lbl_Comp' + j).text(parseInt($('#compCode' + i).val()))
            $('#lbl_CPlnQtybol' + j).text(parseInt($('#compPlannedQty' + i).val()))


        }
    }
    $("#lbl_PlnQtybol").text(sumOfPlannedQty);
    $("#lbl_ActQtybol").text(sumOfActualQty);
    //lbl_Compidbol2
    //lbl_CPlnQtybol2
    //lbl_CActQtybol2
}
function loadSlipPressed() {
    for (k = 1; k < 9; k++) {
        $('#lbl_Comp' + k).text(" ");
        $('#lbl_CompPQty' + k).text(" ");
    }

    var countTr = $('#compTable tr').length;
    var j = 0;
    for (var i = 0; i < countTr; i++) {
        if ($.isNumeric(parseInt($('#compPlannedQty' + i).val()))) {
            j++;
            $('#lbl_Comp' + j).text(parseInt($('#compCode' + i).val()))
            $('#lbl_CompPQty' + j).text(parseInt($('#compPlannedQty' + i).val()))


        }
    }

}
function closePressed() {
    //$("#shipmentStatusId").val(5);

}
(function (factory) {
    if (typeof define === 'function' && define.amd) {
        // AMD. Register as an anonymous module.
        define(['jquery'], factory);
    } else if (typeof exports === 'object') {
        // Node/CommonJS style for Browserify
        module.exports = factory;
    } else {
        // Browser globals: jQuery or jQuery-like library, such as Zepto
        factory(window.jQuery || window.$);
    }
}(function ($) {

    var ticksTo1970 = (((1970 - 1) * 365 + Math.floor(1970 / 4)
                        - Math.floor(1970 / 100)
                        + Math.floor(1970 / 400)) * 24 * 60 * 60 * 10000000);

    var formatDateTime = function (format, date, settings) {
        var output = '';
        var literal = false;
        var iFormat = 0;

        // Check whether a format character is doubled
        var lookAhead = function (match) {
            var matches = (iFormat + 1 < format.length
                           && format.charAt(iFormat + 1) == match);
            if (matches) {
                iFormat++;
            }
            return matches;
        };

        // Format a number, with leading zero if necessary
        var formatNumber = function (match, value, len) {
            var num = '' + value;
            if (lookAhead(match)) {
                while (num.length < len) {
                    num = '0' + num;
                }
            }
            return num;
        };

        // Format a name, short or long as requested
        var formatName = function (match, value, shortNames, longNames) {
            return (lookAhead(match) ? longNames[value] : shortNames[value]);
        };

        // Get the value for the supplied unit, e.g. year for y
        var getUnitValue = function (unit) {
            switch (unit) {
                case 'y': return date.getFullYear();
                case 'm': return date.getMonth() + 1;
                case 'd': return date.getDate();
                case 'g': return date.getHours() % 12 || 12;
                case 'h': return date.getHours();
                case 'i': return date.getMinutes();
                case 's': return date.getSeconds();
                case 'u': return date.getMilliseconds();
                default: return '';
            }
        };

        for (iFormat = 0; iFormat < format.length; iFormat++) {
            if (literal) {
                if (format.charAt(iFormat) == "'" && !lookAhead("'")) {
                    literal = false;
                }
                else {
                    output += format.charAt(iFormat);
                }
            } else {
                switch (format.charAt(iFormat)) {
                    case 'a':
                        output += date.getHours() < 12
                            ? settings.ampmNames[0]
                            : settings.ampmNames[1];
                        break;
                    case 'd':
                        output += formatNumber('d', date.getDate(), 2);
                        break;
                    case 'S':
                        var v = getUnitValue(iFormat && format.charAt(iFormat - 1));
                        output += (v && (settings.getSuffix || $.noop)(v)) || '';
                        break;
                    case 'D':
                        output += formatName('D',
                                             date.getDay(),
                                             settings.dayNamesShort,
                                             settings.dayNames);
                        break;
                    case 'o':
                        var end = new Date(date.getFullYear(),
                                           date.getMonth(),
                                           date.getDate()).getTime();
                        var start = new Date(date.getFullYear(), 0, 0).getTime();
                        output += formatNumber(
                            'o', Math.round((end - start) / 86400000), 3);
                        break;
                    case 'g':
                        output += formatNumber('g', date.getHours() % 12 || 12, 2);
                        break;
                    case 'h':
                        output += formatNumber('h', date.getHours(), 2);
                        break;
                    case 'u':
                        output += formatNumber('u', date.getMilliseconds(), 3);
                        break;
                    case 'i':
                        output += formatNumber('i', date.getMinutes(), 2);
                        break;
                    case 'm':
                        output += formatNumber('m', date.getMonth() + 1, 2);
                        break;
                    case 'M':
                        output += formatName('M',
                                             date.getMonth(),
                                             settings.monthNamesShort,
                                             settings.monthNames);
                        break;
                    case 's':
                        output += formatNumber('s', date.getSeconds(), 2);
                        break;
                    case 'y':
                        output += (lookAhead('y')
                                   ? date.getFullYear()
                                   : (date.getYear() % 100 < 10 ? '0' : '')
                                   + date.getYear() % 100);
                        break;
                    case '@':
                        output += date.getTime();
                        break;
                    case '!':
                        output += date.getTime() * 10000 + ticksTo1970;
                        break;
                    case "'":
                        if (lookAhead("'")) {
                            output += "'";
                        } else {
                            literal = true;
                        }
                        break;
                    default:
                        output += format.charAt(iFormat);
                }
            }
        }
        return output;
    };

    $.fn.formatDateTime = function (format, settings) {
        settings = $.extend({}, $.formatDateTime.defaults, settings);

        this.each(function () {
            var date = $(this).attr(settings.attribute);

            // Use explicit format string first,
            // then fallback to format attribute
            var fmt = format || $(this).attr(settings.formatAttribute);

            if (typeof date === 'undefined' || date === false) {
                date = $(this).text();
            }

            if (date === '') {
                $(this).text('');
            } else {
                $(this).text(formatDateTime(fmt, new Date(date), settings));
            }
        });

        return this;
    };

    /**
       Format a date object into a string value.
       The format can be combinations of the following:
       a - Ante meridiem and post meridiem
       d  - day of month (no leading zero)
       dd - day of month (two digit)
       o  - day of year (no leading zeros)
       oo - day of year (three digit)
       D  - day name short
       DD - day name long
       g  - 12-hour hour format of day (no leading zero)
       gg - 12-hour hour format of day (two digit)
       h  - 24-hour hour format of day (no leading zero)
       hh - 24-hour hour format of day (two digit)
       u  - millisecond of second (no leading zeros)
       uu - millisecond of second (three digit)
       i  - minute of hour (no leading zero)
       ii - minute of hour (two digit)
       m  - month of year (no leading zero)
       mm - month of year (two digit)
       M  - month name short
       MM - month name long
       S  - ordinal suffix for the previous unit
       s  - second of minute (no leading zero)
       ss - second of minute (two digit)
       y  - year (two digit)
       yy - year (four digit)
       @  - Unix timestamp (ms since 01/01/1970)
       !  - Windows ticks (100ns since 01/01/0001)
       '...' - literal text
       '' - single quote

       @param  format    string - the desired format of the date
       @param  date      Date - the date value to format
       @param  settings  Object - attributes include:
           ampmNames        string[2] - am/pm (optional)
           dayNamesShort    string[7] - abbreviated names of the days
                                        from Sunday (optional)
           dayNames         string[7] - names of the days from Sunday (optional)
           monthNamesShort  string[12] - abbreviated names of the months
                                         (optional)
           monthNames       string[12] - names of the months (optional)
           getSuffix        function(num) - accepts a number and returns
                                            its suffix
           attribute        string - Attribute which stores datetime, defaults
                                     to data-datetime, only valid when called
                                     on dom element(s). If not present,
                                     uses text.
           formatAttribute  string - Attribute which stores the format, defaults
                                     to data-dateformat.
       @return  string - the date in the above format
    */
    $.formatDateTime = function (format, date, settings) {
        settings = $.extend({}, $.formatDateTime.defaults, settings);
        if (!date) { return ''; }
        return formatDateTime(format, date, settings);
    };

    $.formatDateTime.defaults = {
        monthNames: ['January', 'February', 'March', 'April', 'May', 'June',
                     'July', 'August', 'September', 'October', 'November',
                     'December'],
        monthNamesShort: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul',
                          'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
        dayNames: ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday',
                   'Friday', 'Saturday'],
        dayNamesShort: ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'],
        ampmNames: ['AM', 'PM'],
        getSuffix: function (num) {
            if (num > 3 && num < 21) {
                return 'th';
            }

            switch (num % 10) {
                case 1: return "st";
                case 2: return "nd";
                case 3: return "rd";
                default: return "th";
            }
        },
        attribute: 'data-datetime',
        formatAttribute: 'data-dateformat'
    };

}));

