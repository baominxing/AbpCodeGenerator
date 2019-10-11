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
    baseUrl: window.location.href,
    noneErrorCode: "0",
    key_SqlServerConnectionStrig: "SqlServerConnectionStrig"
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
