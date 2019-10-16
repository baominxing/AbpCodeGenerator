// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

//Make sure jQuery has been loaded before wimi.js
if (typeof jQuery === "undefined") {
    throw new Error("site requires jQuery");
}

//Make sure layer has been loaded before wimi.js
if (typeof layer === "undefined") {
    throw new Error("site requires layer");
}

// Write your JavaScript code.
var site = site || {};

site.app = site.app || {};

site.app.name = "";
site.app.ico = "";
site.app.copyright = "";

site.app.setting = {
    service: {
        baseUrl: window.location.href,
        noneErrorCode: "0"
    },
    localStorageKey: {
        Home: "Home"
    }
};

site.app.notify = {
    success: function (message) {
        //墨绿深蓝风
        layer.open({
            type: 1,
            skin: 'layui-layer-molv',
            shade: 0, //不显示遮罩
            btn: 0,
            closeBtn: 0,
            offset: "rb",
            time: 2000,
            content: '<div style="padding: 20px 80px;">' + message + '</div>'
        });
    },

    warn: function () {

    },

    error: function (message) {
        //墨绿深蓝风
        layer.alert(
            message,
            {
                skin: 'layui-layer-molv',
                closeBtn: 0
            });
    }
};

site.app.setBusy = function (message) {
    //注意，layer.msg默认3秒自动关闭，如果数据加载耗时比较长，需要设置time
    return layer.msg(message, { icon: 16, shade: 0.1, shadeClose: false, time: 60000 });
};

site.app.closeBusy = function (obj) {
    layer.close(obj);
};

$.validator.setDefaults({
    // 错误插入位置，以及插入的内容
    errorPlacement: function (error, element) {
        $(element).addClass("error");
    },
    submitHandler: function () {
    },
    // 验证成功后调用的方法
    success: function (element) {
        $(element).removeClass("error");
    }
});

// 序列化表单元素为json对象(按name匹配)
$.fn.serializeFrom2JsonObjViaName = function () {
    var o = { "unique_id": new Date().getTime(), "state": false };
    var a = this.serializeArray();
    $.each(a, function () {
        if (o[this.name]) {
            if (!o[this.name].push) {
                o[this.name] = [o[this.name]];
            }
            o[this.name].push(this.value || '');
        } else {
            o[this.name] = this.value || '';
        }
    });
    return o;
};

// 将json对象序列化自动填充到表单元素中(按name匹配)
$.fn.serializeJsonObj2FomViaName = function (o) {
    var $from = this;
    var a = $from.serializeArray();

    $.each(a, function () {
        if (o[this.name]) {
            $from.find("#" + this.name).val(o[this.name]);
        } else {
            $from.find("#" + this.name).val("");
        }
    });
};