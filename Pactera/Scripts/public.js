// 获取查询参数部分
function getQueryString() {
    var result = location.search.match(new RegExp("[\?\&][^\?\&]+=[^\?\&]+", "g"));
    for (var i = 0; i < result.length; i++) {
        result[i] = result[i].substring(1);
    }
    return result;
}
// 根据QueryString参数名称获取值
function getQueryStringByName(name) {
    var result = location.search.match(new RegExp("[\?\&]" + name + "=([^\&]+)", "i"));
    if (result == null || result.length < 1) {
        return "";
    }
    return result[1];
}
// 根据QueryString参数索引获取值
function getQueryStringByIndex(index) {
    if (index == null) {
        return "";
    }
    var queryStringList = getQueryString();
    if (index >= queryStringList.length) {
        return "";
    }
    var result = queryStringList[index];
    var startIndex = result.indexOf("=") + 1;
    result = result.substring(startIndex);
    return result;
}
//格式化时间控件
function dataformatter(date) {
    var y = date.getFullYear();
    var m = date.getMonth() + 1;
    var d = date.getDate();
    return y + '-' + (m < 10 ? ('0' + m) : m) + '-' + (d < 10 ? ('0' + d) : d);
}
//时间控件值
function dataparser(s) {
    if (!s) return new Date();
    var ss = (s.split('-'));
    var y = parseInt(ss[0], 10);
    var m = parseInt(ss[1], 10);
    var d = parseInt(ss[2], 10);
    if (!isNaN(y) && !isNaN(m) && !isNaN(d)) {
        return new Date(y, m - 1, d);
    } else {
        return new Date();
    }
}
//页面form验证
$.extend($.fn.validatebox.defaults.rules, {
    minLength: {
        validator: function (value, param) {
            return value.length >= param[0];
        },
        message: '请输入至少{0}个字符。'
    },
    maxLength: {
        validator: function (value, param) {
            return value.length <= param[0];
        },
        message: '最多允许输入{0}个字符。'
    },
    intx: {
        validator: function (value) {
            var reg = new RegExp('^[0-9]*$');
            return reg.test(value);
        },
        message: '请输入正确的数字。'
    },
    number: {
        validator: function (value) {
            var reg = new RegExp('^[0-9]+(\\.[0-9]+)?$');
            return reg.test(value);
        },
        message: '请输入正确的数字。'
    },
    tel: {
        validator: function (value) {
            //var reg = new RegExp('^\\d+-?\\d+$');
            //var reg = /^\d+-?\d+$/;
            var reg =  /^\d+(-\d+(-\d+)?)?$/;
            return reg.test(value);
        },
        message: '请输入正确的电话。'
    }
});

//获取当前日期
function getCurrTime() {
    var date = new Date();
    var y = date.getFullYear();
    var m = date.getMonth() + 1;
    var d = date.getDate();
    var dd = y + '-' + (m < 10 ? ('0' + m) : m) + '-' + (d < 10 ? ('0' + d) : d);
    return dd;
}
