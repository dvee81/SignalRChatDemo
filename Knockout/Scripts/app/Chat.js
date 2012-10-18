$(function() {

    var demo = $.connection.demo;

     //View Bindings
    window.rvm = {};
    rvm.UserName = ko.observable();
    rvm.Password = ko.observable();
    
    window.svm = {};
    svm.UserName = ko.observable();
    svm.Password = ko.observable();

    window.mvm = {};
    mvm.messages = ko.observableArray([]);
    
    ko.applyBindings(mvm);
    ko.applyBindings(rvm);
    ko.applyBindings(svm);

    //converts a ko view model to javascript
    var koViewModelToJs = function(kovm) {
            var pojo = {};
            for (var key in kovm) {
                pojo[key] = typeof kovm[key] == 'function' ? kovm[key]() : kovm[key];
            }
            return pojo;
        };
    
    var setCookie = function(cName, value, exdays) {
            var exdate = new Date();
            exdate.setDate(exdate.getDate() + exdays);
            var c_value = escape(value) + ((exdays == null) ? "" : "; expires=" + exdate.toUTCString());
            document.cookie = cName + "=" + c_value;
        };

    var getCookie = function(c_name) {
            var i, x, y, ARRcookies = document.cookie.split(";");
            for (i = 0; i < ARRcookies.length; i++) {
                x = ARRcookies[i].substr(0, ARRcookies[i].indexOf("="));
                y = ARRcookies[i].substr(ARRcookies[i].indexOf("=") + 1);
                x = x.replace(/^\s+|\s+$/g, "");
                if (x == c_name) {
                    return unescape(y);
                }
            }
        };
        
    demo.sendMeMessage = function(name, message) {
        mvm.messages.push({
            name:name,
            message:message
        });
    };

    demo.broadcast = function(name,message) {
        mvm.messages.push({
            name:name,
            message:message
        });
    };

    $.connection.hub.start(function() {});

    demo.setCredentials = function(username) {
        if(!getCookie('username'))
         setCookie('username', username);
        
        $('#panelAuthenticate').hide();
        $('#panelAuthenticated').show()
        $('#userNameLabel').text(username);

        $('#textMessage').removeAttr('readonly');
        
       if($('#modalRegister').length > 0)
            $('#modalRegister').hide();

       if ($('#modalLogin').length > 0)
            $('#modalLogin').hide();

       if ($('.modal-backdrop').length > 0)
            $('.modal-backdrop').hide();
    };

    //Invoked every times the user creates a new connection
    demo.resumeUserSession = function() {
        var name = getCookie('username');
        if (name) {
            demo.createUserSession({
                UserName: name
            });
        } else {
            demo.disconnect();
        }
    };

    var sendMessage = function () {
        var name = getCookie('username');
        if (!name) {
            alert('Please sign in');
            return false;
        }
        var message = $('#textMessage').val();
        if (!message)
            return false;

        demo.send(name, message).fail(function (e) {
            alert(e);
        });

        $('#textMessage').val('');
        $('textMessage').focus();
    };

    //User Interactions
    $('#btnSendMessage').click(function() {
        sendMessage();
    });

    $('#textMessage').live("keyup", function (e) {
        if (e.keyCode == 13) {
            sendMessage();
        }
    });

    $('#registrationLink').click(function() {
        $('#modalRegister').modal({
            backdrop: true,
            keyboard: true
        });
    });

    $('#signInLink').click(function () {

        $('#modalLogin').modal({
            backdrop: true,
            keyboard: true
        });
    });

    $('#btnLogin').click(function () {
        var pojo = koViewModelToJs(svm);
        demo.signIn(pojo);
    });

    $('#btn-register').click(function() {
        var pojo = koViewModelToJs(rvm);
        demo.createUserSession(pojo);
    });

    $('#logoutLink').click(function () {
        setCookie('username', null, -1);
        window.location = 'index.html';
    });

});