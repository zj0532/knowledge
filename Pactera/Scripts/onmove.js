//$.extend($.fn.window.methods, {
//    // 接管onDrag事件
//    dragInWindow: function (arg) {
//        var t = $.data(arg[0], "window"),
//			dragOpts = t.window.draggable("options"),
//			oldDrag = dragOpts.onDrag;
//        // 重写拖拽中的处理
//        dragOpts.onDrag = function (e) {
//            var d = e.data, win = $(window),
//				w = win.width() - t.proxy.outerWidth(),
//				h = win.height() - t.proxy.outerHeight();
//            // 可拖拽的区域Check
//            d.left = d.left < 0 ? 0 : d.left >= w ? w : d.left,
//			d.top = d.top < 0 ? 0 : d.top >= h ? h : d.top;
//            return oldDrag(e);
//        }
//    }
//});




var easyuiPanelOnMove = function (left, top) {
    var parentObj = $(this).panel('panel').parent();
    //if (left < 0) {
    //    $(this).window('move', {
    //        left: 1
    //    });
    //}
    //if (top < 0) {
    //    $(this).window('move', {
    //        top: 1
    //    });
    //}
    var width = $(this).panel('options').width;
    var height = $(this).panel('options').height;
    var right = left + width;
    var buttom = top + height;
    var parentWidth = parentObj.width();
    var parentHeight = parentObj.height();
    //debugger;
    if (right > parentWidth) {
        $(this).window('move', {
            left: parentWidth - width
        });
    }
    if (buttom > parentHeight) {
        $(this).window('move', {
            top: parentHeight - height
        });
    }
    if (parentObj.css("overflow") == "hidden") {
        if (left > parentWidth - width) {
            $(this).window('move', {
                "left": parentWidth - width
            });
        }
        if (top > parentHeight - height) {
            $(this).window('move', {
                "top": parentHeight - height
            });
        }
    }
};
$.fn.panel.defaults.onMove = easyuiPanelOnMove;
$.fn.window.defaults.onMove = easyuiPanelOnMove;
$.fn.dialog.defaults.onMove = easyuiPanelOnMove;