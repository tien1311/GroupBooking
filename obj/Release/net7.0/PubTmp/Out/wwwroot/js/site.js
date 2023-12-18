
(function ($) {


    var loading = $('#loading-overlay');
    // Sự kiện khi yêu cầu Ajax bắt đầu
    $(document).ajaxStart(function () {
        loading.show();
    });

    // Sự kiện khi yêu cầu Ajax hoàn tất
    $(document).ajaxComplete(function () {
        loading.hide();
    });


    //Lưu data Airport vào trình duyệt
    function Save_Browser_Airport() {
        $.ajax({
            url: '/Home/List_Json_Airports', // Đường dẫn đến action
            type: 'GET',
            dataType: 'json',
            async: true, // Thực hiện bất đồng bộ
            success: function (data) {
                // Lưu dữ liệu vào localStorage
                localStorage.setItem('list_airport', JSON.stringify(data));
                // Lưu dữ liệu vào localStorage session storage
                //sessionStorage.setItem("list_airport", JSON.stringify(data));           
            },
            error: function (error) {
                console.error('Error:', error);
            },
            complete: function () {
                // Hàm này sẽ được gọi sau khi request hoàn tất, bất kể thành công hay thất bại
                loadDataDropdownAirport();
            }
        });
    };
    Save_Browser_Airport();


    function loadDataDropdownAirport()
    {
        // Đọc dữ liệu từ session storage
        // var list_airport = sessionStorage.getItem("list_airport");
        // Đọc dữ liệu từ session storage localStorage
        var list_airport = localStorage.getItem("list_airport");
        list_airport = JSON.parse(list_airport);
        //Get dữ liệu trong localStorage và gán vào dropdown
        if (list_airport) {
            $.each(list_airport, function (key, val) {
                var gt = val["IataCode"];
                var fromData = {
                    value: val["IataCode"],
                    text: val["CityName"] + ' (' + val["IataCode"] + ')'
                };
                if (gt === "SGN") {
                    fromData.selected = true;
                }
                var toData = {
                    value: val["IataCode"],
                    text: val["CityName"] + ' (' + val["IataCode"] + ')'
                };

                if (gt === "HAN") {
                    toData.selected = true;
                }
                $("#dropdown_departure_code").append($('<option>', fromData));
                $("#dropdown_arrival_code").append($('<option>', toData));
            });
            $('#dropdown_departure_code').dropdown();
            $('#dropdown_arrival_code').dropdown();
        }
        else {
            localStorage.setItem("list_airport", "true");
        }
    }
    

    var btn__search__form = $("#btn__search__form");
    btn__search__form.submit(function (e) {
        e.preventDefault();
        var data = $(this).serialize();
        var url = btn__search__form.attr("action");    

        // Phân giải đối tượng serialized thành một đối tượng JavaScript
        var form__search_object = {};
        data.split('&').forEach(function (pair) {
            pair = pair.split('=');
            form__search_object[pair[0]] = decodeURIComponent(pair[1] || '');
        });
        form__search_object.Departure = dropdown_departure_code.dropdown('get text');
        form__search_object.Arrival = dropdown_arrival_code.dropdown('get text');
        // Truy cập giá trị của trường DepartureCode
        // var departureCodeValue = form__search_object.DepartureCode;
        // Tạo thành chuỗi json
        var search_obj = JSON.stringify(form__search_object);
        // Lưu vào sessionStorage
        sessionStorage.setItem("search_obj", search_obj);

        // Sử dụng moment.js để chuyển đổi ngày tháng
        // var formattedDate = moment(form__search_object.ArrivalDate, "DD-MM-YYYY").format("YYYY-MM-DD");
        $.ajax({
            type: "post",
            url: url,
            data: { jsonString: search_obj },
            async: true, // Thực hiện bất đồng bộ
            success: function (response) {
                // Thêm dữ liệu vào #partialContainer
                $("#partialContainer").html(response);
            }
        });

    });


    // Lắng nghe sự kiện submit cho các phần tử con của #partialContainer
    $(document).on('submit', '#form__booking', function (e) {
        e.preventDefault();

        var search_obj = sessionStorage.getItem('search_obj');
        search_obj = JSON.parse(search_obj);
        // Phân giải đối tượng serialized thành một đối tượng JavaScript
        var form__search_object = {};
        var data = $(this).serialize();
        data.split('&').forEach(function (pair) {
            pair = pair.split('=');
            form__search_object[pair[0]] = decodeURIComponent(pair[1] || '');
        });
        form__search_object.Id = search_obj.Id;
        form__search_object.BookType = search_obj.BookType;
        if (search_obj.numPerson == null) {
            form__search_object.Person = (parseInt(search_obj.adult) + parseInt(search_obj.child) + parseInt(search_obj.baby));
        }
        else {
            form__search_object.Person = (parseInt(search_obj.numPerson));
        }
        var url = $('#form__booking').attr('action');
        $.ajax({
            type: "post",
            url: url,
            data: { data: JSON.stringify(form__search_object) },
            success: function (response) {
                $("#body__booking__modal").html("<p>" + response.msg + "<p>");
                $("#msg__booking_modal").modal({
                    centered: false
                }).modal('show');
            }
        });
    });


    // Lấy ngày hiện tại
    var currentDate = new Date();


    // Định dạng ngày theo "DD-MM-YYYY" bằng moment.js
    var formattedDate_From = moment(currentDate).format('YYYY-MM-DD');
    var formattedDate_To = moment(currentDate).format('YYYY-MM-DD');


    // Lấy giá trị tham số của search_obj trong trình duyệt
    var search_params = JSON.parse(sessionStorage.getItem("search_obj"));
    

    if (search_params != null)
    {
        if (search_params.BookType == "ROUNDTRIP") {               
            formattedDate_From = moment(search_params.DepartureDate, "DD-MM-YYYY").format("YYYY-MM-DD");
            formattedDate_To = moment(search_params.ArrivalDate, "DD-MM-YYYY").format("YYYY-MM-DD");
        }
        if (search_params.BookType == "ONEWAY") {
            formattedDate_From = moment(search_params.DepartureDate, "DD-MM-YYYY").format("YYYY-MM-DD");
        }
    }   

    
    var start_Calendar = $('#rangestart');
    var end_Calendar = $('#rangeend');
 
    start_Calendar.calendar({
        type: 'date',
        formatter: {
            date: 'DD-MM-YYYY'
        },
        text: {
            days: ['CN', 'T2', 'T3', 'T4', 'T5', 'T6', 'T7'],
            months: [
                'Tháng 1', 'Tháng 2', 'Tháng 3', 'Tháng 4', 'Tháng 5', 'Tháng 6',
                'Tháng 7', 'Tháng 8', 'Tháng 9', 'Tháng 10', 'Tháng 11', 'Tháng 12'
            ],
            monthsShort: [
                'Th1', 'Th2', 'Th3', 'Th4', 'Th5', 'Th6',
                'Th7', 'Th8', 'Th9', 'Th10', 'Th11', 'Th12'
            ]
        },
        initialDate: formattedDate_From,
        endCalendar: end_Calendar,
        minDate: new Date(),
        maxDate: new Date(new Date().getFullYear() + 1, 11, 31)
    });

    end_Calendar.calendar({
        type: 'date',
        formatter: {
            date: 'DD-MM-YYYY'
        },
        text: {
            days: ['CN', 'T2', 'T3', 'T4', 'T5', 'T6', 'T7'],
            months: [
                'Tháng 1', 'Tháng 2', 'Tháng 3', 'Tháng 4', 'Tháng 5', 'Tháng 6',
                'Tháng 7', 'Tháng 8', 'Tháng 9', 'Tháng 10', 'Tháng 11', 'Tháng 12'
            ],
            monthsShort: [
                'Th1', 'Th2', 'Th3', 'Th4', 'Th5', 'Th6',
                'Th7', 'Th8', 'Th9', 'Th10', 'Th11', 'Th12'
            ]
        },
        initialDate: formattedDate_To,
        startCalendar: start_Calendar,
        minDate: new Date(),
        maxDate: new Date(new Date().getFullYear() + 1, 11, 31)
    });

    
    //Radio type chuyến bay
    var oneway = $("#oneway");
    var roundtrip = $("#roundtrip");
    var ArrivalDate = $("#ArrivalDate");

    roundtrip.prop("checked", true);
    if (search_params != null) {
        if (search_params.BookType == "ONEWAY") {
            oneway.prop("checked", true);
            //Vô hiệu hóa input ngày về
            ArrivalDate.prop("disabled", true);
            //Clear ngày đã nhập trước đó
            end_Calendar.calendar('clear');
            //Vô hiệu hóa lịch ngày về
            end_Calendar.calendar('destroy');
        }
        if (search_params.BookType == "ROUNDTRIP") {
            roundtrip.prop("checked", true);
        }
    }


    $("input[name='BookType']").change(function () {
        if (roundtrip.prop("checked") == true) {
            //Bật input ngày về
            ArrivalDate.prop("disabled", false);
            //Bật lịch ngày về
            end_Calendar.calendar({
                type: 'date',
                formatter: {
                    date: 'DD-MM-YYYY'
                },
                text: {
                    days: ['CN', 'T2', 'T3', 'T4', 'T5', 'T6', 'T7'],
                    months: [
                        'Tháng 1', 'Tháng 2', 'Tháng 3', 'Tháng 4', 'Tháng 5', 'Tháng 6',
                        'Tháng 7', 'Tháng 8', 'Tháng 9', 'Tháng 10', 'Tháng 11', 'Tháng 12'
                    ],
                    monthsShort: [
                        'Th1', 'Th2', 'Th3', 'Th4', 'Th5', 'Th6',
                        'Th7', 'Th8', 'Th9', 'Th10', 'Th11', 'Th12'
                    ]
                },
                initialDate: formattedDate_To,
                startCalendar: start_Calendar,
                minDate: new Date(),
                maxDate: new Date(new Date().getFullYear() + 1, 11, 31),
                startCalendar: start_Calendar
            });
        }
        if (oneway.prop("checked") == true) {
            roundtrip.prop("checked", false);
            //Vô hiệu hóa input ngày về
            ArrivalDate.prop("disabled", true);
            //Clear ngày đã nhập trước đó
            end_Calendar.calendar('clear');
            //Vô hiệu hóa lịch ngày về
            end_Calendar.calendar('destroy');
        }    
    });


    //Ẩn hiển div số hành khách
    var input__person = $("#person");
    var input__adult = $('#adult');
    var input__child = $('#child');
    var input__baby = $('#baby');
    var num__person = $(".num__person");

    function set_value_person(val1, val2, val3) {
        var num__person__result = val1 + " Người lớn, " + val2 + " Trẻ em, " + val3 + " Em bé"; 
        input__person.val(num__person__result);   
        input__adult.val(val1);
        input__child.val(val2);
        input__baby.val(val3);
    };

    if (search_params != null) {
        var adult = search_params.adult;
        var child = search_params.child;
        var baby = search_params.baby;
        set_value_person(adult, child, baby);
    }
    else {
        set_value_person(1, 0, 0);    
    }
    
    
    input__person.click(function (e) {
        // Hiển thị hoặc ẩn div khi input được nhấp
        num__person.toggle();
        e.stopPropagation(); // Ngăn sự kiện click truyền ra ngoài
    });
    num__person.click(function (e) {
        // Hiển thị div trong vùng con của nó
        e.stopPropagation(); // Ngăn sự kiện click truyền ra ngoài
    });

    $(document).click(function (event) {
        if (event.target !== input__person[0] && event.target !== num__person[0]) {
            // Ẩn div khi click ra bên ngoài input hoặc div
            num__person.hide();
        }
    });

   
    var proQty = $(document).find(".pro-qty");
    proQty.on('click', '.adult', function () {
        var $button = $(this);
        var oldValue = $button.parent().find('input').val();
        var child = $(document).find("#child").val();
        var baby = $(document).find("#baby").val();
        if ($button.hasClass('inc')) {
            var newVal = parseFloat(oldValue) + 1;
        } else {
            //Don't allow decrementing below zero
            if (oldValue > 1) {
                var newVal = parseFloat(oldValue) - 1;
            }
            else {
                newVal = 1;
            }
        }
        $button.parent().find('input').val(newVal);
        set_value_person(newVal, child, baby);
    });

    proQty.on('click', '.child', function () {
        var $button = $(this);
        var oldValue = $button.parent().find('input').val();
        var adult = $(document).find("#adult").val();
        var baby = $(document).find("#baby").val();
        if ($button.hasClass('inc')) {
            var newVal = parseFloat(oldValue) + 1;
        } else {
            //Don't allow decrementing below zero
            if (oldValue > 0) {
                var newVal = parseFloat(oldValue) - 1;
            }
            else {
                newVal = 0;
            }
        }
        $button.parent().find('input').val(newVal);
        set_value_person(adult, newVal, baby);
    });

    proQty.on('click', '.baby', function () {
        var $button = $(this);
        var oldValue = $button.parent().find('input').val();
        var adult = $(document).find("#adult").val();
        var child = $(document).find("#child").val();
        if ($button.hasClass('inc')) {
            var newVal = parseFloat(oldValue) + 1;
        } else {
            //Don't allow decrementing below zero
            if (oldValue > 0) {
                var newVal = parseFloat(oldValue) - 1;
            }
            else {
                newVal = 0;
            }
        }
        $button.parent().find('input').val(newVal);
        set_value_person(adult, child, newVal);
    });


    //Hiển thị modal chi tiết chuyến bay
    //$(".__detail__").click(function () {
    //    var detail = $(this).attr("id");      
    //    $("#__"+detail+"__").modal({
    //        centered: false
    //    }).modal('show');
    //});


    //Khi sử dụng Ajax để load partialView ta phải sử dụng delegation để kích hoạt sự kiện
    // Sự kiện delegation cho phần tử có id là "#partialContainer"
    $(document).on("click", "#partialContainer .__detail__", function () {
        // Xử lý sự kiện cho nút được nhấp
        var detail = $(this).attr("id");
        $("#__" + detail + "__").modal({
            centered: false
        }).modal('show');
    });


    //Hiển thị modal booking chuyến bay
    //$(".btn__booking__buy").click(function () {
    //    var Id = $(this).attr("id");
    //    var search_obj = sessionStorage.getItem('search_obj');
    //    if (search_obj != null) {
    //        search_obj = JSON.parse(search_obj);
    //        search_obj.Id = Id;
    //        sessionStorage.setItem('search_obj', JSON.stringify(search_obj));
    //    }
    //    $("#booking__modal").modal({
    //        centered: false
    //    }).modal('show');
    //});


    //Khi sử dụng Ajax để load partialView ta phải sử dụng delegation để kích hoạt sự kiện
    // Sự kiện delegation cho phần tử có id là "#partialContainer"
    $(document).on("click", "#partialContainer .btn__booking__buy", function () {
        
        // Xử lý sự kiện cho nút được nhấp
        var Id = $(this).attr("id");
        var BookType = $(this).attr("booktype");
        var Person = $(this).attr("person");
       
        $("#send_id_booking").val(Id);
        $("#send_id_booktype").val(BookType);
        $("#send_id_numberOfpassengers").val(Person);
        var search_obj = sessionStorage.getItem('search_obj');
        if (search_obj != null) {
            search_obj = JSON.parse(search_obj);
            search_obj.Id = Id;
            sessionStorage.setItem('search_obj', JSON.stringify(search_obj));
        }
        $.ajax({
            type: "post",
            url: '/Home/booking',
            data: { jsonString: JSON.stringify(search_obj)},
            async: true, // Thực hiện bất đồng bộ
            success: function (response) {
                // Thêm dữ liệu vào #partialContainer
                $("#partialContainer").html(response);  
                fillAtribute();
                addRow();
                form_Validation();
            }
        });
        //$("#booking__modal").modal({
        //    centered: false
        //}).modal('show');
        ////Gọi validation form sau khi bật popup
        //form_Validation();
    });



    //Input xem theo tháng
    var ViewOfMonth = $("#ViewOfMonth");
    ViewOfMonth.click(function () {
        var isChecked = ViewOfMonth.prop("checked");
        if (isChecked) {
            ViewOfMonth.val(1);
        } else {
            ViewOfMonth.val(0);
        }
    });

    var dropdown__airline = $('#dropdown__airline');
    var selected__airline = $("#selected__airline");
    dropdown__airline.dropdown({
        keepSearchTerm: true,
        onChange: function (value, text, $selectedItem) {
            // Lấy giá trị và đặt nó vào ô văn bản
            selected__airline.val(value);
            // Lấy danh sách giá trị đã chọn
            var selectedValues = dropdown__airline.dropdown('get value');
            // Lưu giá trị đã chọn vào localStorage
            localStorage.setItem('dropdown__airline', JSON.stringify(selectedValues));    
        }
    });
    // Lấy giá trị đã lưu trữ từ localStorage 
    var dropdown__airline__value = localStorage.getItem('dropdown__airline'); 
    if (dropdown__airline__value) {
        // Chuyển đổi chuỗi JSON thành mảng giá trị
        var parsedValues = JSON.parse(dropdown__airline__value);
        // Nếu có giá trị đã lưu trữ, đặt giá trị cho dropdown
        dropdown__airline.dropdown('set selected', parsedValues);
    }


    //Dropdown nơi đi
    var dropdown_departure_code = $('#dropdown_departure_code');
    var selected_departure_code = $("#selected_departure_code");
    dropdown_departure_code.dropdown({
        //Khởi tạo dropdown
        ignoreDiacritics: true,
        sortSelect: true,
        fullTextSearch: 'exact', // Tìm kiếm chính xác
        //fullTextSearch: true // Tìm kiếm toàn văn bản
        //match: 'text', // Tìm kiếm theo văn bản,
        onChange: function (value, text, $selectedItem) {
            // Lấy giá trị và đặt nó vào ô văn bản
            selected_departure_code.val(value);
            // Lưu giá trị đã chọn vào localStorage
            localStorage.setItem('dropdown_departure_code', $(this).val());       
            localStorage.setItem('dropdown_departure', text); 
        }
    });
    // Lấy giá trị đã lưu trữ từ localStorage 
    var departure_code_value = localStorage.getItem('dropdown_departure_code');
    // Nếu có giá trị đã lưu trữ, đặt giá trị cho dropdown
    dropdown_departure_code.dropdown('set selected', departure_code_value);


    //Dropdown nơi đến
    var dropdown_arrival_code = $('#dropdown_arrival_code');
    var selected_arrival_code = $("#selected_arrival_code");
    dropdown_arrival_code.dropdown({
        // Khởi tạo dropdown
        ignoreDiacritics: true,
        sortSelect: true,
        fullTextSearch: 'exact', // Tìm kiếm chính xác
        //fullTextSearch: true // Tìm kiếm toàn văn bản
        //match: 'text', // Tìm kiếm theo văn bản
        onChange: function (value, text, $selectedItem) {
            // Lấy giá trị và đặt nó vào ô văn bản
            selected_arrival_code.val(value);
            // Lưu giá trị đã chọn vào localStorage
            localStorage.setItem('dropdown_arrival_code', $(this).val());  
            localStorage.setItem('dropdown_arrival', text);  
        }
    });
    // Lấy giá trị đã lưu trữ từ localStorage 
    var arrival_code_value = localStorage.getItem('dropdown_arrival_code');
    // Nếu có giá trị đã lưu trữ, đặt giá trị cho dropdown
    dropdown_arrival_code.dropdown('set selected', arrival_code_value);
   

    //Thông tin form booking & tin hành khách
    $('.accordion__person')
        .accordion();


    function form_Validation() {
        $('.ui.form')
        .form({
            on: 'blur',
            //inline: true, // Hiển thị lỗi tại input
            fields: {
                empty: {
                    identifier: 'empty',
                    rules: [
                        {
                            type: 'empty',
                            prompt: 'Vui lòng điền vào trường này'
                        }
                    ]
                },
                dropdown: {
                    identifier: 'dropdown',
                    rules: [
                        {
                            type: 'empty',
                            prompt: 'Vui lòng chọn một giá trị'
                        }
                    ]
                },
                checkbox: {
                    identifier: 'checkbox',
                    rules: [
                        {
                            type: 'checked',
                            prompt: 'Vui lòng chọn một giá trị'
                        }
                    ]
                },
                Email: {
                    identifier: 'Email',
                    rules: [
                        {
                            type: 'email',
                            prompt: 'Vui lòng nhập email hợp lệ'
                        }
                    ]
                },
                Phone: {
                    identifier: 'Phone',
                    rules: [
                        {
                            type: 'regExp[/^0/]',
                            prompt: 'Số điện thoại bắt đầu bằng số 0'
                        },
                        {
                            type: 'integer[0,9]',
                            prompt: 'Số điện thoại phải là số'
                        },
                        {
                            type: 'minLength[10]',
                            prompt: 'Số điện thoại có ít nhất 10 số'
                        },
                        {
                            type: 'maxLength[11]',
                            prompt: 'Số điện thoại có tối đa 11 số'
                        }
                    ]
                }
            }
        });
    }
    


    $('.date__shared').calendar({
        type: 'date',
        formatter: {
            date: 'DD-MM-YYYY'
        },
        text: {
            days: ['CN', 'T2', 'T3', 'T4', 'T5', 'T6', 'T7'],
            months: [
                'Tháng 1', 'Tháng 2', 'Tháng 3', 'Tháng 4', 'Tháng 5', 'Tháng 6',
                'Tháng 7', 'Tháng 8', 'Tháng 9', 'Tháng 10', 'Tháng 11', 'Tháng 12'
            ],
            monthsShort: [
                'Th1', 'Th2', 'Th3', 'Th4', 'Th5', 'Th6',
                'Th7', 'Th8', 'Th9', 'Th10', 'Th11', 'Th12'
            ]
        }
    });


    var dropdown__document__type = $('#dropdown__document__type');
    var selected__document__type = $('#selected__document__type');

    dropdown__document__type.change(function () {
        var selectedValue = $(this).val();
        selected__document__type.val(selectedValue);
    });

    var dropdown__holder = $('#dropdown__holder');
    var selected__holder = $('#selected__holder');

    dropdown__holder.change(function () {
        var selectedValue = $(this).val();
        selected__holder.val(selectedValue);
    });


    $(document).on('click', '.date__selected', function (e) {
        e.preventDefault();
        var r1_departure_date = $(this).attr("r1");
        var search_obj = sessionStorage.getItem("search_obj");
        search_obj = JSON.parse(search_obj);
        search_obj.ViewOfMonth = "0";
        ViewOfMonth.prop("checked", false);
        if (search_obj.BookType == "ONEWAY") {
            // Chuyển đổi thành đối tượng moment
            var r1_momentDate = moment(r1_departure_date, "M-D-YYYY HH:mm:ss");
            // Định dạng ngày theo "dd-MM-yyyy"
            var r1_formattedDate = r1_momentDate.format("DD-MM-YYYY");
            search_obj.DepartureDate = r1_formattedDate;             
        }
        else
        {
            var r2_arrival_date = $(this).attr("r2");
            // Chuyển đổi thành đối tượng moment
            var r1_momentDate = moment(r1_departure_date, "M-D-YYYY HH:mm:ss");
            var r2_momentDate = moment(r2_arrival_date, "M-D-YYYY HH:mm:ss");
            // Định dạng ngày theo "dd-MM-yyyy"
            var r1_formattedDate = r1_momentDate.format("DD-MM-YYYY");
            var r2_formattedDate = r2_momentDate.format("DD-MM-YYYY");
            search_obj.DepartureDate = r1_formattedDate;  
            search_obj.ArrivalDate = r2_formattedDate;    
        }
        sessionStorage.setItem("search_obj", JSON.stringify(search_obj)); 
        $.ajax({
            type: "post",
            url: "Home/LoadPartialDataSearch_New",
            data: { jsonString: JSON.stringify(search_obj) },
            async: true, // Thực hiện bất đồng bộ
            success: function (response) {
                // Thêm dữ liệu vào #partialContainer
                $("#partialContainer").html(response);
            }
        });
        
    });


    function fillAtribute() {
        var search_obj = sessionStorage.getItem('search_obj');
        search_obj = JSON.parse(search_obj);
        var num = parseInt(search_obj.adult) + parseInt(search_obj.child) + parseInt(search_obj.baby);
        for (var i = 1; i <= num; i++) {
            var rangeBirthday = $(document).find("#rangeBirthday" + i.toString());
            rangeBirthday.calendar({
                type: 'date',
                formatter: {
                    date: 'DD-MM-YYYY'
                },
                text: {
                    days: ['CN', 'T2', 'T3', 'T4', 'T5', 'T6', 'T7'],
                    months: [
                        'Tháng 1', 'Tháng 2', 'Tháng 3', 'Tháng 4', 'Tháng 5', 'Tháng 6',
                        'Tháng 7', 'Tháng 8', 'Tháng 9', 'Tháng 10', 'Tháng 11', 'Tháng 12'
                    ],
                    monthsShort: [
                        'Th1', 'Th2', 'Th3', 'Th4', 'Th5', 'Th6',
                        'Th7', 'Th8', 'Th9', 'Th10', 'Th11', 'Th12'
                    ]
                }
            });
            $('#dropdown__passengers__title' + i).dropdown({
                onChange: function (value, text, $selectedItem) {
                    var selectedValue = $(this).attr("data");
                    $('#' + selectedValue).val(value);
                }
            });
        }

        $(document).find('#passengersInfo').on('click', '.btn__delete__row', function () {
            var search_obj = sessionStorage.getItem('search_obj');
            search_obj = JSON.parse(search_obj);
            var num = parseInt(search_obj.numPerson);
            if (search_obj.numPerson == null) {
                num = (parseInt(search_obj.adult) + parseInt(search_obj.child) + parseInt(search_obj.baby)) - 1;
            }
            else
            {
                if (parseInt(search_obj.numPerson) > 1)
                {
                    num = parseInt(search_obj.numPerson) - 1;
                }
            }
           
            search_obj.numPerson = num;
            sessionStorage.setItem("search_obj", JSON.stringify(search_obj)); 
            // Lấy số lượng hàng hiện có trong bảng
            var rowCount = $(document).find('#passengersInfo #passengersBody tr').length;
            //alert(rowCount);
            // Xóa hàng ở cuối bảng
            //if (rowCount > 1) {
            //    $('#passengersInfo tbody tr:last').remove();
            //}
            // Xóa hàng ở vị trí index
            if (rowCount > 9) {
                var currentRow = $(this).closest('tr');
                var rowIndex = currentRow.index() + 1;
                currentRow.remove();            
            }
            if (num > 0) {
                updateTable(num);
            }
            // Cập nhật lại số thứ tự ở cột đầu tiên
            //updateRowNumbers();
            });
            // Hàm cập nhật lại số thứ tự ở cột đầu tiên
            //function updateRowNumbers() {
            //    $('#passengersInfo #passengersBody tr').each(function (index) {
            //        $(this).find('td:first').text(index + 1);
            //    });
            //}    
            updateTable(num);
        }


    function addRow() {
        var btn__add__row = $(document).find('#passengersInfo').next();
        btn__add__row.on('click', '.btn__add__row', function () {
            var search_obj = sessionStorage.getItem('search_obj');
            search_obj = JSON.parse(search_obj);
            var num = parseInt(search_obj.numPerson) + 1;
            if (search_obj.numPerson == null) {
                num = (parseInt(search_obj.adult) + parseInt(search_obj.child) + parseInt(search_obj.baby)) + 1;
            }         
            search_obj.numPerson = num;
            sessionStorage.setItem('search_obj', JSON.stringify(search_obj));
            var newRowHtml =
                '<tr>' +
                '<td class="ip__info__name">' +
                '<input type="text" name="fullname' + num + '" class="first__name__booking">' +
                '</td>' +
                '<td class="ip__info__title">' +
                '<input type="text" name="title' + num + '" id="selected__passengers__title'+num+'" value="MR" hidden>' +
                '<select class="ui fluid dropdown" id="dropdown__passengers__title' + num + '" data="selected__passengers__title' + num + '">' +
                '<option value="MR">MR (Quý Ông)</option>' +
                '<option value="MRS">MRS (Quý Bà)</option>' +
                '<option value="MS">MS (Quý Cô)</option>' +
                '<option value="MISS">MISS (Bé Gái)</option>' +
                '<option value="MSTR">MSTR (Bé Trai)</option>' +
                '</select>' +
                '</td>' +
                '<td class="ip__info__date">' +
                '<div class="ui calendar" id="rangeBirthday' + num + '">' +
                '<div class="ui input left icon">' +
                '<i class="calendar icon"></i>' +
                '<input type="text" name="dateOfBirth' + num + '">' +
                '</div>' +
                '</div>' +
                '</td>' +
                '<td>' +
                '<button type="button" class="mini ui icon button btn__delete__row">' +
                '<i class="trash alternate icon"></i>' +
                '</button>' +
                '</td>' +
                '</tr>';

            // Thêm hàng mới vào cuối bảng
            var table = $(document).find('#passengersInfo #passengersBody');
            table.append(newRowHtml);
            // Lấy phần tử mới thêm vào (ví dụ: hàng cuối cùng của bảng)
            var newRow = $(document).find('#passengersInfo #passengersBody tr:last');

            // Gọi lại calendar trên phần tử mới
            newRow.find('#rangeBirthday' + num).calendar({
                type: 'date',
                formatter: {
                    date: 'DD-MM-YYYY'
                },
                text: {
                    days: ['CN', 'T2', 'T3', 'T4', 'T5', 'T6', 'T7'],
                    months: [
                        'Tháng 1', 'Tháng 2', 'Tháng 3', 'Tháng 4', 'Tháng 5', 'Tháng 6',
                        'Tháng 7', 'Tháng 8', 'Tháng 9', 'Tháng 10', 'Tháng 11', 'Tháng 12'
                    ],
                    monthsShort: [
                        'Th1', 'Th2', 'Th3', 'Th4', 'Th5', 'Th6',
                        'Th7', 'Th8', 'Th9', 'Th10', 'Th11', 'Th12'
                    ]
                }
            });
            // Gọi lại dropdown trên phần tử mới
            $('#dropdown__passengers__title' + num).dropdown({
                onChange: function (value, text, $selectedItem) {
                    var selectedValue = $(this).attr("data");
                    $('#' + selectedValue).val(value);
                }
            });
            updateTable(num);
        });
    }  

    function updateTable(num) {
        // Cập nhật table 
        var rowFirst = $(document).find('.scroll-table-info #numberOfGuests');    
        rowFirst.text(num);
        var rowTwo = $(document).find('.scroll-table-info #priceOfTrip');       
        var total = parseFloat(rowTwo.text().replace(/,/g, '')) * num;
        var formattedTotal = total.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
        var rowLast = $(document).find('.scroll-table-info #priceTotal');
        rowLast.text(formattedTotal);
        var rowThree = $(document).find('.scroll-table-info #priceAgent'); 
        var saleOf = rowThree.attr('data');  
        var saleOfValue = total - parseFloat(saleOf.replace(/,/g, '')) * num;
        var formattedSaleOf = saleOfValue.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
        var totalSaleOf = total - saleOfValue;
        var formattedTotalSaleOf = totalSaleOf.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
        if (num >= 10) {
            // Giá giảm
            rowThree.text(formattedSaleOf);
            // Tổng thành tiền giảm      
            rowLast.text(formattedTotalSaleOf);
        }
        else
        {
            rowThree.text("0");
        }
      

       

    }


    // Hàm bỏ dấu tiếng việt và chuyển chữ thường thành hoa
    function removeDiacriticsAndToUpperCase(input) {
        // Bảng chữ cái có dấu
        var diacriticsMap = {
            'á': 'a', 'à': 'a', 'ả': 'a', 'ã': 'a', 'ạ': 'a',
            'ă': 'a', 'ắ': 'a', 'ằ': 'a', 'ẳ': 'a', 'ẵ': 'a', 'ặ': 'a',
            'â': 'a', 'ấ': 'a', 'ầ': 'a', 'ẩ': 'a', 'ẫ': 'a', 'ậ': 'a',
            'đ': 'd',
            'é': 'e', 'è': 'e', 'ẻ': 'e', 'ẽ': 'e', 'ẹ': 'e',
            'ê': 'e', 'ế': 'e', 'ề': 'e', 'ể': 'e', 'ễ': 'e', 'ệ': 'e',
            'í': 'i', 'ì': 'i', 'ỉ': 'i', 'ĩ': 'i', 'ị': 'i',
            'ó': 'o', 'ò': 'o', 'ỏ': 'o', 'õ': 'o', 'ọ': 'o',
            'ô': 'o', 'ố': 'o', 'ồ': 'o', 'ổ': 'o', 'ỗ': 'o', 'ộ': 'o',
            'ơ': 'o', 'ớ': 'o', 'ờ': 'o', 'ở': 'o', 'ỡ': 'o', 'ợ': 'o',
            'ú': 'u', 'ù': 'u', 'ủ': 'u', 'ũ': 'u', 'ụ': 'u',
            'ư': 'u', 'ứ': 'u', 'ừ': 'u', 'ử': 'u', 'ữ': 'u', 'ự': 'u',
            'ý': 'y', 'ỳ': 'y', 'ỷ': 'y', 'ỹ': 'y', 'ỵ': 'y',
            'Á': 'A', 'À': 'A', 'Ả': 'A', 'Ã': 'A', 'Ạ': 'A',
            'Ă': 'A', 'Ắ': 'A', 'Ằ': 'A', 'Ẳ': 'A', 'Ẵ': 'A', 'Ặ': 'A',
            'Â': 'A', 'Ấ': 'A', 'Ầ': 'A', 'Ẩ': 'A', 'Ẫ': 'A', 'Ậ': 'A',
            'Đ': 'D',
            'É': 'E', 'È': 'E', 'Ẻ': 'E', 'Ẽ': 'E', 'Ẹ': 'E',
            'Ê': 'E', 'Ế': 'E', 'Ề': 'E', 'Ể': 'E', 'Ễ': 'E', 'Ệ': 'E',
            'Í': 'I', 'Ì': 'I', 'Ỉ': 'I', 'Ĩ': 'I', 'Ị': 'I',
            'Ó': 'O', 'Ò': 'O', 'Ỏ': 'O', 'Õ': 'O', 'Ọ': 'O',
            'Ô': 'O', 'Ố': 'O', 'Ồ': 'O', 'Ổ': 'O', 'Ỗ': 'O', 'Ộ': 'O',
            'Ơ': 'O', 'Ớ': 'O', 'Ờ': 'O', 'Ở': 'O', 'Ỡ': 'O', 'Ợ': 'O',
            'Ú': 'U', 'Ù': 'U', 'Ủ': 'U', 'Ũ': 'U', 'Ụ': 'U',
            'Ư': 'U', 'Ứ': 'U', 'Ừ': 'U', 'Ử': 'U', 'Ữ': 'U', 'Ự': 'U',
            'Ý': 'Y', 'Ỳ': 'Y', 'Ỷ': 'Y', 'Ỹ': 'Y', 'Ỵ': 'Y'
        };

        // Loại bỏ dấu và chuyển đổi thành chữ thường thành hoa
        var normalizedString = input.replace(/[^\u0000-\u007E]/g, function (char) {
            return diacriticsMap[char] || char;
        }).toUpperCase();

        return normalizedString;
    }


    var first__name__booking = $(document).find('.first__name__booking');
    var first__name__return = "";
    $(document).on('input', '#partialContainer .first__name__booking', function () {
        // Lắng nghe sự kiện input cho phần tử input
        first__name__booking = $(this);
        // Bỏ dấu và chuyển đổi chuỗi thành viết Hoa
        first__name__return = removeDiacriticsAndToUpperCase(first__name__booking.val());
    });

    $(document).on('blur', '#partialContainer .first__name__booking', function () {
        // Khôi phục giá trị sau khi di chuyển chuột khỏi input
        first__name__booking.val(first__name__return);
    });

})(jQuery);

