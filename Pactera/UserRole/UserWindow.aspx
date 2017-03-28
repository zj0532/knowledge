<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserWindow.aspx.cs" Inherits="Pactera.UserRole.UserWindow" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>

    <title>添加用户</title>

    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />

    <link href="../Scripts/jquery-easyui-1.4.1/themes/default/easyui-pactera.css" rel="stylesheet" />
    <link href="../Scripts/jquery-easyui-1.4.1/themes/icon.css" rel="stylesheet" />

    <script src="../Scripts/jquery-easyui-1.4.1/jquery.min.js"></script>
    <script src="../Scripts/jquery-easyui-1.4.1/jquery.easyui.min.js"></script>
    <script src="../Scripts/jquery-easyui-1.4.1/locale/easyui-lang-zh_CN.js"></script>
    
</head>
<body style="padding:0px; margin:0px;">
    <div style="padding: 5px;overflow:hidden">
        <div style="height:40px;padding-left:40px;"><a style="color:red;font-size:14px;font-weight:bold;">*为必填项</a></div>
        <div style="height:35px;">
            <span style="width:100px;height:30px;line-height:30px;text-align:right;display:block;float:left;">区域：</span>
            <input class="easyui-combobox" id="oneArea" />&nbsp;
            <input class="easyui-combobox" id="twoArea" />&nbsp;
            <input class="easyui-combobox" id="threeArea" /><a style="color:red;">*</a>
        </div>
        <div style="height:35px;">
            <span style="width:100px;height:30px;line-height:30px;text-align:right;display:block;float:left;">用户名：</span>
            <input type="text" class="easyui-textbox" id="txbUserName" style="width: 160px;" data-options="required: true,validType:'minLength[5]'" /><a style="color:red;">*</a>
        </div>
        <div style="height:35px;">
            <span style="width:100px;height:30px;line-height:30px;text-align:right;display:block;float:left;">密码：</span>
            <input type="password" class="easyui-textbox" id="txbpassword" style="width: 160px;" data-options="required: true" /><a style="color:red;">*</a><br />
        </div>
        <div style="height:35px;">
            <span style="width:100px;height:30px;line-height:30px;text-align:right;display:block;float:left;">重复密码：</span>
            <input type="password" class="easyui-textbox" id="Repassword" style="width: 160px;" data-options="required: true" /><a style="color:red;">*</a><br />
        </div>
        <div style="height:35px;">
            <span style="width:100px;height:30px;line-height:30px;text-align:right;display:block;float:left;">姓名：</span>
            <input type="text" class="easyui-textbox" id="txbName" style="width: 160px;" data-options="required: true" /><a style="color:red;">*</a><br />
        </div>
        <div style="height:35px;">
            <span style="width:100px;height:30px;line-height:30px;text-align:right;display:block;float:left;">联系电话：</span>
            <input type="text" class="easyui-textbox" id="txbPhone" style="width: 160px;" data-options="required:true"  /><a style="color:red;">*联系电话格式：0532-88937370</a><br />
        </div>
        <div style="height:35px;">
            <span style="width:100px;height:30px;line-height:30px;text-align:right;display:block;float:left;">手机：</span>
            <input type="text" class="easyui-textbox" id="txbMobile" style="width: 160px;" data-options="required:true" /><a style="color:red;">*</a><br />
        </div>
        <div style="height:35px;">
            <span style="width:100px;height:30px;line-height:30px;text-align:right;display:block;float:left;">邮箱：</span>
            <input type="text" class="easyui-textbox" id="txbEmail" data-options="validType:'email',required:true" style="width: 160px;" /><a style="color:red;">*</a><br />
        </div>
        <div style="height:35px;">
            <span style="width:100px;height:30px;line-height:30px;text-align:right;display:block;float:left;">办公地址：</span>
            <input type="text" class="easyui-textbox" id="txbAddress" style="width: 160px;" data-options="required:true" /><a style="color:red;">*</a><br />
        </div>
        <div style="height:35px;">
            <span style="width:100px;height:30px;line-height:30px;text-align:right;display:block;float:left;">状态：</span>
            <input type="text" class="easyui-combobox" id="txbState" style="width: 160px;" /><br />
        </div>
        
        <div style="text-align: left; padding: 10px;">
            <a id="linkConfimation" class="linkbutton">添加</a>
        </div>
    </div>

    <script type="text/javascript">

        function oneArea() {
            $("#oneArea").combobox({
                panelHeight: "auto",
                width:"120px",
                editable: false,
                valueField: "Id",
                textField: "Text",
                url: "/Handler/Area/PublicArea.ashx?Action=AreaJilian&ParentId=0",
                onSelect: function (record) {
                    twoArea(record["Id"]);
                    $("#threeArea").combobox("clear");
                }
            });
        }
        function twoArea(parentId) {
            $("#twoArea").combobox({
                panelHeight: "auto",
                width: "120px",
                editable: false,
                valueField: "Id",
                textField: "Text",
                url: "/Handler/Area/PublicArea.ashx?Action=AreaJilian&ParentId=" + parentId,
                onSelect: function (record) {
                    threeArea(record["Id"]);
                }
            });
        }
        function threeArea(parentId) {
            $("#threeArea").combobox({
                panelHeight: "auto",
                width: "120px",
                editable: false,
                valueField: "Id",
                textField: "Text",
                url: "/Handler/Area/PublicArea.ashx?Action=AreaJilian&ParentId=" + parentId
            });
        }

        $(document).ready(function () {
            $("#twoArea").combobox({
                width: "120px",
            })
            $("#threeArea").combobox({
                width: "120px",
            })
            oneArea();
            $("#txbState").combobox({
                data: [
                    //{ value: "-1", text: "请选择", "selected": true },
                    { value: "0", text: "使用" },
                    { value: "1", text: "过期" }
                ],
                panelHeight: "auto",
                editable: false,
                valueField: "value",
                textField: "text",
                onLoadSuccess: function () {
                    // 这时候集合中没有这个，就会直接显示这个，但是在下拉中缺没有这个选项，利用BUG来实现默认值
                    $("#txbState").combobox("setValue", "0");
                    $("#txbState").combobox("setText", "使用");
                }
            });
        });
        // 窗口 - 确认按钮
        $("#linkConfimation").linkbutton({
            iconCls: "icon-ok",
            onClick: function () {
                // 在这里获取你输入的用户信息
                var userName = $("#txbUserName").textbox("getValue");
                var password = $("#txbpassword").textbox("getValue");
                var repassword = $("#Repassword").textbox("getValue");
                var name = $("#txbName").textbox("getValue");
                var phone = $("#txbPhone").textbox("getValue");
                var mobile = $("#txbMobile").textbox("getValue");
                var email = $("#txbEmail").textbox("getValue");
                var address = $("#txbAddress").textbox("getValue");
                var enable = $("#txbState").combobox("getValue");
                var oneArea = $("#oneArea").combobox("getValue");
                var twoArea = $("#twoArea").combobox("getValue");
                var threeArea = $("#threeArea").combobox("getValue");
                var params = {
                    "Action": "CreateNewUser","OneArea":oneArea,"TwoArea":twoArea,"ThreeArea":threeArea, "UserName": userName, "Password": password,
                    "Name": name, "Phone": phone, "Mobile": mobile, "Repassword": repassword, "Email": email, "Address": address, "Enable": enable
                };

                // 添加用户
                $.post("/Handler/PublicUser.ashx", params, function (data) {
                    if (data["StateCode"] == 0) {
                        // 这里的状态码是自己在后台定义的，就统一0是成功，其他是失败
                        parent.$.messager.alert("提示", data["Message"], "info", function () {
                            // 这里的function是指点了Easyui对话框的确定时执行
                            parent.closeAndDestroyModalWindow(1);
                        });
                    } else {
                        // 如果没有操作成功，提示对应的错误信息，但是不关闭窗口
                        parent.$.messager.alert("提示", data["Message"], "info");
                    }
                }, "json");
            }

        });


    </script>

</body>
</html>
