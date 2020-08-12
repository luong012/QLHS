$(document).ready(function() {

    $('#register').hide();

    $('#confirm').click(function() {
        $('#register').show();
        if ($('#customer').val() == "0") {
            $.ajax({
                type: "GET",
                url: "/current-customer",
                data: {
                    username: $('#account').text()
                },
                success: function(response) {
                    // response.CUS_BIRTH = new Date(response.CUS_BIRTH);
                    // moment.locale();
                    var date = moment(response.CUS_BIRTH).format('YYYY-MM-DD')
                        // var date = moment(, "yyyy-MM-dd");
                    console.log(date);
                    $('#fullname').val(response.CUS_NAME);
                    $('#gender').val(response.CUS_GENDER);
                    $('#id').val(response.CUS_ID);
                    $('#email').val(response.CUS_EMAIL);
                    $('#address').val(response.CUS_ADDRESS);
                    $('#phone').val(response.CUS_PHONE);
                    $('#birthday').val(date);

                }
            });

            $('#notify').click(function() {
                if ($('#checkin').val() != "" && $('#checkout').val() != "") {
                    $.ajax({
                        type: "GET",
                        url: "/send-email",
                        data: {
                            username: $('#account').text()
                        },
                        success: function(response) {
                            if (response == "oke") {
                                $.notify("Order compelete ! Please check your email for additional information", {
                                    animate: {
                                        enter: 'animated fadeInRight',
                                        exit: 'animated fadeOutRight'
                                    }
                                });
                            } else
                            if (response == "no") {
                                $.notify("Errors !! ", {
                                    animate: {
                                        enter: 'animated fadeInRight',
                                        exit: 'animated fadeOutRight'
                                    }
                                });
                            }
                        }
                    });
                } else {
                    alert("Fill out Checkin date and Checkout Date !!!");
                    window.location = '/';
                }
            })
        } else {
            $('#fullname').val('');
            $('#gender').val('');
            $('#id').val('');
            $('#email').val('');
            $('#address').val('');
            $('#phone').val('');
            $('#birthday').val('');

            $('#notify').click(function() {
                if ($('#checkin').val() != "" && $('#checkout').val() != "") {

                    $.ajax({
                        type: "GET",
                        url: "/admin/new-customer",
                        data: {
                            fullname: $('#fullname').val(),
                            gender: $('#gender').val(),
                            id: $('#id').val(),
                            email: $('#email').val(),
                            address: $('#address').val(),
                            phone: $('#phone').val(),
                            birthday: $('#birthday').val(),

                        },
                        success: function(response) {
                            console.log(response);
                            if (response == "oke") {
                                $.notify("Order compelete ! Please check your email for additional information", {
                                    animate: {
                                        enter: 'animated fadeInRight',
                                        exit: 'animated fadeOutRight'
                                    }
                                });
                            } else
                            if (response == "no") {
                                $.notify("Errors !! ", {
                                    animate: {
                                        enter: 'animated fadeInRight',
                                        exit: 'animated fadeOutRight'
                                    }
                                });
                            }

                        }
                    });



                } else {
                    alert("Fill out Checkin date and Checkout Date !!!");
                    window.location = '/';
                }
            })

        }

    })



})