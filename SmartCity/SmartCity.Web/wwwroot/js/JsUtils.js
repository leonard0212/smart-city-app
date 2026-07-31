"use strict";
(function () {
    var appUrl = $("#appUrl").val();
    function createGuid() {
        return ([1e7] + -1e3 + -4e3 + -8e3 + -1e11).replace(/[018]/g, c =>
            (c ^ crypto.getRandomValues(new Uint8Array(1))[0] & 15 >> c / 4).toString(16)
        );
    }


    function getElementSize($element) {
        var h = $element.height();
        var w = $element.width();
        return { height: h, width: w };
    }


    function getData($trigger, url, params, asynchronous, successCallback, failCallback, completeCallback) {
        var queryString = params
            ? url + (url.indexOf("?") < 0 ? "?" : "&") + $.param(params).split("%5B%5D").join("")
            : url;

        ajaxCall("GET", $trigger, queryString, null, asynchronous, successCallback, failCallback, completeCallback);
    }

    function postJsonData($trigger, url, data, asynchronous, successCallback, failCallback, completeCallback) {
        return ajaxCall("POST", $trigger, url, data, asynchronous, successCallback, failCallback, completeCallback);
    }

    function ajaxCall(type, $trigger, url, data, asynchronous, successCallback, failCallback, completeCallback) {
        $("#error-text-container").hide();
        $("#error-warning-container").hide();
        var start = new Date().getTime();
        var ajaxOptions = {
            type: type,
            url: url,
            async: asynchronous,
            success: function (response, textStatus, jqXhr) {
                if (response && response.success || jqXhr.status === 200)
                    if (successCallback)
                        successCallback(response);
                    else if (failCallback)
                        failCallback(response);
            },
            error: function (jqXhr) {

                onAjaxError(jqXhr, $trigger);

                if (failCallback)
                    failCallback(response);
            },
            complete: function (jqXhr) {
                var end = new Date().getTime();
                onAjaxComplete(jqXhr, $trigger);
               
                if (completeCallback)
                    completeCallback(response);
            },
        };

        if (data && type !== "GET") {

            ajaxOptions.data = data;


            var isFormData = data instanceof FormData;
            if (isFormData) {
                ajaxOptions.processData = false;
                ajaxOptions.contentType = false;

            }
        
        }

        if ($trigger) {

            $trigger.addClass("btn-loading");

            var loader = $trigger.attr('id');
        
            if (loader == "app-loader-container")
                $trigger.show();

        }

        return $.ajax(ajaxOptions);
    }

    function onAjaxError(jqXhr, $btnTrigger) {

        var response = parseJson(jqXhr.responseText);

        if ($btnTrigger) {
            $btnTrigger.removeClass("btn-loading");
            var loader = $btnTrigger.attr('id');
            if (loader == "app-loader-container")
                $btnTrigger.hide();
        }

        if (jqXhr.status === 500)

            showInternalError(jqXhr);
        if (jqXhr.status === 404)
            showErrorAlert("error", "error");
        else if (jqXhr.status === 403)
            showErrorAlert("error", "error");
        else if (response === null)
            showErrorAlert("error", "error");
    }

    function onAjaxComplete(jqXhr, $btnTrigger) {

        var response = parseJson(jqXhr.responseText);

        if (response && response.errors && response.errors.length > 0) {
            showErrorAlert("Error", response.errors[0]);
        }

        if ($btnTrigger) {
            $btnTrigger.removeClass("btn-loading");
            var loader = $btnTrigger.attr('id');
            if (loader == "app-loader-container")
                $btnTrigger.hide();
        }

    }



    function showErrorAlert(error, stace) {



    }
    function showInternalError(jqXhr) {

        $("#error-text-container").text(jqXhr.responseText);
        $("#error-warning-container").show();

    }


    function parseJson(text) {
        var response;
        try {
            response = JSON.parse(text);
        }
        catch (e) {
            response = null;
        }

        return response;
    }


    function serializeObject($form) {
        var obj = {};
        var array = $form.serializeArray();

        array = array.map(function (elem) {
            elem.type = $("[name='" + elem.name + "']:first", $form).attr("type");
            return elem;
        })
            .filter(function (elem, index) {
                return elem.type !== "checkbox" || index === 0 || elem.name !== array[index - 1].name;
            });

        $.each(array, function () {
            if (obj[this.name] !== undefined) {
                if (!obj[this.name].push) {
                    obj[this.name] = [obj[this.name]];
                }
                obj[this.name].push(this.value || "");
            }
            else {
                obj[this.name] = this.value || "";
            }
        });

        return obj;
    }



    function initAjax() {

        var appUrl = $("#appUrl").val();
        var loginUrl = "auth/login";
        var windowsUrl = appUrl + loginUrl;
        var redirectToLogin = function (xhr) {

            window.location = windowsUrl;
        }
        $.ajaxSetup({
            statusCode: {
                403: redirectToLogin,
                401: redirectToLogin
            }
        });
    }


  

    function scrollToLastElement(container, delay, scroolto, guid) {
        setTimeout(function () {

            var newElement = $(".flow-spec-exec[data-id='" + guid + "']");
            var elementAboveCodeMirror = newElement.parent().prevAll(".CodeMirror").first();

            //var lastInstanceParent = $(".parentItem .codemirror-instance").last();
            //var nextElement = lastInstanceParent.next();
            //var lastCodeMirrorScroll = nextElement.children(".CodeMirror-scroll").first();
            //var lastCodeMirrorSizer = lastCodeMirrorScroll.children().first();

            if (elementAboveCodeMirror.length > 0) {
                var scrollToTop = scroolto;
                $(container).scrollTop(scrollToTop);
            }

        }, delay);
    }

    var cursorInterval;

    function facusToLastElement(delay, guid) {
        setTimeout(function () {

            var newElement = $(".flow-spec-exec[data-id='" + guid + "']");
            var elementAboveCodeMirror = newElement.parent().prevAll(".CodeMirror").first();

            if (elementAboveCodeMirror.length > 0) {
                elementAboveCodeMirror.addClass("CodeMirror-focused-newLine");
            }

            cursorInterval = setInterval(toggleCursorCodeMirrorNewLine, 700);

        }, delay);
    }

    function toggleCursorCodeMirrorNewLine() {
        var cursor = $(".CodeMirror-focused-newLine div.CodeMirror-cursors").last();
        cursor.toggleClass('cursor-visible');
    }

    function stopCursorAnimation() {
        clearInterval(cursorInterval);
    }


    var jsUtils = {
        createGuid: createGuid,
        getElementSize: getElementSize,
        getData: getData,
        postJsonData: postJsonData,
        onAjaxError: onAjaxError,
        onAjaxComplete: onAjaxComplete,
        serializeObject: serializeObject,
        initAjax: initAjax,
        scrollToLastElement: scrollToLastElement,
        facusToLastElement: facusToLastElement,
        stopCursorAnimation: stopCursorAnimation
    };
    window.jsUtils = jsUtils;

})();