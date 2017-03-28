<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AreaManage.aspx.cs" Inherits="Pactera.Area.AreaManage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>区域信息管理</title>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />

    <link href="../Css/common.css" rel="stylesheet" />
    <link href="../Scripts/jquery-easyui-1.4.1/themes/default/easyui-pactera.css" rel="stylesheet" />
    <link href="../Scripts/jquery-easyui-1.4.1/themes/icon.css" rel="stylesheet" />

    <script src="../Scripts/jquery-easyui-1.4.1/jquery.min.js"></script>
    <script src="../Scripts/jquery-easyui-1.4.1/jquery.easyui.min.js"></script>
    <script src="../Scripts/jquery-easyui-1.4.1/locale/easyui-lang-zh_CN.js"></script>

    <script type="text/javascript">
        // 这里创建了一个变量，记录了当前正在编辑的节点。
        var editNode = null;

        var args = { "Action": "CurrentSigninUserHasPermission" };
        $(document).ready(function () {

            // 初始化树获取数据
            $("#AreaTree").tree({
                url: "/Handler/Area/PublicArea.ashx?Action=GetAreaListJson",
                onSelect: function (node) {
                    $("#BtnAppendNode").linkbutton("enable");
                    // 在这里设置按钮是否启用
                    if (node["Level"] >= 3) {
                        // 如果是第三级，则不允许再新增子集
                        $("#BtnAppendNode").linkbutton("disable");
                    }
                },
                onAfterEdit: function (node) {
                    
                    // 显示更新进度条
                    parent.$.messager.progress({
                        title: "正在处理",
                        msg: "请稍后..."
                    });

                    //console.info(node);
                    // 修改之后进行编辑
                    $.post("/Handler/Area/PublicArea.ashx", { "Action": "UpdateNode", "Id": node["id"], "Text": node["text"] }, function (data) {
                        // 这里返回的是字符串，需要把字符串转化成对象
                        var response = eval("(" + data + ")");

                        // 如果操作没有成功，提示错误信息
                        if (response["StateCode"] != 0) {
                            parent.$.messager.alert("提示", response["Message"], "info");
                            return;
                        }

                        parent.$.messager.progress('close');
                    });
                    
                }
            });

            // 按钮 - 插入同级节点节点
            $("#BtnInsertNode").linkbutton({
                iconCls: "icon-add",
                disabled: true,
                onClick: function () {

                    // 1. 先找到目标节点
                    var node = $("#AreaTree").tree("getSelected");
                    if (node == null) {
                        parent.$.messager.alert("提示", "请选择节点", "info");
                        return;
                    }

                    //// 2. 取消当前的编辑
                    //if (editNode != null) {
                    //    $("#AreaTree").tree("cancelEdit");
                    //}

                    // 2.1 在执行插入之前，显示进度条
                    parent.$.messager.progress({
                        title: "正在处理",
                        msg: "请稍后..."
                    });

                    // 3. 调用后台，获取插入后的节点信息
                    $.post("/Handler/Area/PublicArea.ashx", { "Action": "InsertNodeAndReturn", "ParentId": node["ParentId"], "Level": node["Level"] }, function (data) {

                        // 这里返回的是字符串，需要把字符串转化成对象
                        var response = eval("(" + data + ")");

                        // 如果操作没有成功，提示错误信息
                        if (response["StateCode"] != 0) {
                            parent.$.messager.alert("提示", response["Message"], "info");
                            return;
                        }

                        // 4. 后台插入完成，在前台也实现插入。
                        $("#AreaTree").tree("insert", {
                            after: node.target,
                            data: { "id": response["Id"], "text": response["Text"], "ParentId": response["ParentId"], "Level": response["Level"] }
                        });

                        // 操作完成后，关闭进度条
                        parent.$.messager.progress('close');

                        // 5. 到这时，已完成插入操作，这时前台和后台的节点是同步的，不需要执行保存操作。
                        // 1,2,3,4,5 这就是思路   ????那为什么我刷新还是数据库里没有记录
                    });
                }
            });

            // 根据权限设置按钮是否启用
            args["Permission"] = "Area_Add";
            $.post("/Handler/PublicPermission.ashx", args, function (data) {
                if (data == "0") $("#BtnInsertNode").linkbutton("enable");
            });

            // 按钮 - 插入子集节点节点
            $("#BtnAppendNode").linkbutton({
                iconCls: "icon-add",
                disabled: true,
                onClick: function () {

                    // 先找到目标节点
                    var node = $("#AreaTree").tree("getSelected");

                    // 如果没有选中任何节点，不执行操作
                    if (node == null) {
                        parent.$.messager.alert("提示", "请选择节点！", "info");
                        return;
                    }
                    //// 1.取消当前的所有编辑comeon
                    //if (editNode != null) {
                    //    $("#AreaTree").tree("cancelEdit");
                    //}

                    // 1.1 在执行插入之前，显示进度条
                    parent.$.messager.progress({
                        title: "正在处理",
                        msg: "请稍后..."
                    });

                    // 2. 调用后台，获取插入后的节点信息
                    $.post("/Handler/Area/PublicArea.ashx", { "Action": "InsertNodeAndReturn", "ParentId": node["id"], "Level": node["Level"] + 1 }, function (data) {
                        // 这里返回的是字符串，需要把字符串转化成对象
                        var response = eval("(" + data + ")");
                        // 如果操作没有成功，提示错误信息
                        if (response["StateCode"] != 0) {
                            parent.$.messager.alert("提示", response["Message"], "info");
                            return;
                        }

                        // 到这里操作已经完成，执行页面的新增
                        $("#AreaTree").tree("append", {
                            parent: node.target,
                            data: { "id": response["Id"], "text": response["Text"], "ParentId": response["ParentId"], "Level": response["Level"] }
                        });

                        // 操作完成后，关闭进度条
                        parent.$.messager.progress('close');
                    });
                }
            });

            // 根据权限设置按钮是否启用
            args["Permission"] = "Area_AddChildren";
            $.post("/Handler/PublicPermission.ashx", args, function (data) {
                if (data == "0") $("#BtnAppendNode").linkbutton("enable");
            });

            // 现在做的是同步操作，你懂吗？同步，这个我看了，没发做异步保存，可以做但是很麻烦，我知道$.post(url,param,function(){},"json")这个是吧我昨天试了是麻烦
            // 你明白我做的吗？接着上午的说，你知道是怎么实现的吗？上午的怎么实现的啊
            // 现在新增做完了，需要做修改了

            // 先实现编辑节点
            $("#BtnEditNode").linkbutton({
                iconCls: "icon-edit",
                disabled: true,
                onClick: function () {

                    // 先找到目标节点，这就记录了
                    editNode = $("#AreaTree").tree("getSelected");

                    // 没有选中节点、或者当前有正在编辑的节点，不执行编辑
                    if (editNode == null) {
                        parent.$.messager.alert("提示", "请选择节点！", "info");
                    }
                    // 开始编辑
                    $("#AreaTree").tree("beginEdit", editNode.target);

                }
            });

            // 根据权限设置按钮是否启用
            args["Permission"] = "Area_Edit";
            $.post("/Handler/PublicPermission.ashx", args, function (data) {
                if (data == "0") $("#BtnEditNode").linkbutton("enable");
            });

            //////删除节点
            $("#BtnDelNode").linkbutton({
                iconCls: "icon-remove",
                disabled: true,
                onClick: function () {
                    var deleteNode = $("#AreaTree").tree("getSelected");
                    if (deleteNode == null) {
                        parent.$.messager.alert("提示", "请选择要删除的数据！", "info");
                        return;
                    }
                    //console.info(deleteNode);
                    var Level = deleteNode.Level;
                    var url = "/Handler/Area/PublicArea.ashx";
                    if (Level != 3) {
                        var ChilderenNode = $("#AreaTree").tree("getChildren", deleteNode.target);
                        var Params = { "Action": "DeleteNode", "Id": deleteNode.id, "Level": "Level" };
                    } else {
                        var Params = { "Action": "DeleteNode", "Id": deleteNode.id };
                    }


                    parent.$.messager.confirm("警告", "删除后不可恢复，确认删除？", function (c) {
                        if (!c) return;
                        $.post(url, Params, function (data) {
                            $("#AreaTree").tree("remove", deleteNode.target);
                        });
                    });

                }
            });

            // 根据权限设置按钮是否启用
            args["Permission"] = "Area_Delete";
            $.post("/Handler/PublicPermission.ashx", args, function (data) {
                if (data == "0") $("#BtnDelNode").linkbutton("enable");
            });

            //////插入顶级节点///////
            $("#BtnInsertTopNode").linkbutton({
                iconCls: "icon-add",
                disabled: true,
                onClick: function () {

                    //// 1.取消当前的所有编辑comeon
                    //if (editNode != null) {
                    //    $("#AreaTree").tree("cancelEdit");
                    //}

                    // 1.1 在执行插入之前，显示进度条
                    parent.$.messager.progress({
                        title: "正在处理",
                        msg: "请稍后..."
                    });

                    // 2. 调用后台，获取插入后的节点信息
                    $.post("/Handler/Area/PublicArea.ashx", { "Action": "InsertTopNode", "ParentId": "0", "Level": "1" }, function (data) {
                        // 这里返回的是字符串，需要把字符串转化成对象
                        var response = eval("(" + data + ")");
                        // 如果操作没有成功，提示错误信息
                        if (response["StateCode"] != 0) {
                            parent.$.messager.alert("提示", response["Message"], "info");
                            return;
                        }

                        // 到这里操作已经完成，执行页面的新增
                        $("#AreaTree").tree("append", {
                            data: { "id": response["Id"], "text": response["Text"], "ParentId": "0", "Level": "1" }
                        });

                        // 操作完成后，关闭进度条
                        parent.$.messager.progress('close');
                    });
                }
            });


            // 根据权限设置按钮是否启用
            args["Permission"] = "Area_Top";
            $.post("/Handler/PublicPermission.ashx", args, function (data) {
                if (data == "0") $("#BtnInsertTopNode").linkbutton("enable");
            });

        });

    </script>
</head>
<body>
    <div style="border:1px solid #d0d0d0;width:60%;margin-left:5px;margin-top:5px;">
        <div style="border-bottom:1px solid #d0d0d0;padding-bottom:5px;padding-top:5px;background:#f4f4f4;">
            <a>&nbsp;&nbsp;</a>
            <a id="BtnInsertTopNode">插入顶级节点</a>
            <a id="BtnInsertNode">插入同级节点</a>
            <a id="BtnAppendNode">插入子集节点</a>
            <a id="BtnEditNode">编辑节点</a>
            <a id="BtnDelNode">删除节点</a>
        </div>
        <div style="height:15px;"></div>
    <!-- 页面左右布局，左侧显示树结构的区域信息，可以在树结构中增加子节点和修改当前节点的名称 -->
    <ul id="AreaTree" style="height:480px;padding-left:15px;display:block;"></ul>
    </div>
    <!-- 右侧显示当前节点的一些信息。 -->
</body>
</html>
