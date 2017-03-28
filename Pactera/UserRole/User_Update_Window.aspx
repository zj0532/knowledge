<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="User_Update_Window.aspx.cs" Inherits="Pactera.UserRole.User_Update_Window" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>修改工程师信息</title>
   <%-- <script type="text/javascript">
        var requestData = <%=Request.Form["HiddenIframeRequestDataJson"]%>;
        // 这个requestData就是你再之前页面传递的param，所以你能获取到里面的Id
        alert(requestData["Id"]);
    </script>--%>

    <link href="../Scripts/jquery-easyui-1.4.1/themes/default/easyui-pactera.css" rel="stylesheet" />
    <link href="../Scripts/jquery-easyui-1.4.1/themes/icon.css" rel="stylesheet" />

    <script src="../Scripts/jquery-easyui-1.4.1/jquery.min.js"></script>
    <script src="../Scripts/jquery-easyui-1.4.1/jquery.easyui.min.js"></script>
    <script src="../Scripts/jquery-easyui-1.4.1/locale/easyui-lang-zh_CN.js"></script>

    <script type="text/javascript">
        
        var requestData = <%=Request.Form["HiddenIframeRequestDataJson"]%>;
        //console.info(requestData);
        //console.info(requestData);
        //提交form表单
        function submitUserForm() {
            $("#UpdateUserForm").form("submit", {
                onSubmit: function (param) {
                    param.action = "UpdateUserInfo";
                    param.Id = requestData["Id"];
                    
                    // 这里是获取表单验证，检查表单验证是否通过
                    var isValid = $(this).form("enableValidation").form("validate");
                    if (isValid) {
                        parent.$.messager.progress();// 显示进度条
                    }

                    // 如果返回True就会提交表单，否则不提交表单
                    return isValid;
                },
                success: function (data) {
                    // 表单提交成功隐藏进度条
                    parent.$.messager.progress("close");
                    var result = eval("(" + data + ")");
                    if(result.StateCode != 0){
                        parent.$.messager.alert("提示", result.Message, "info");
                        return;
                    }

                    parent.$.messager.alert("提示", result.Message, "info");
                    parent.closeAndDestroyModalWindow(1);
                }
            });
        }
        
        function oneArea() {
            $("#oneArea").combobox({
                panelHeight: "auto",
                width:"120px",
                editable: false,
                valueField: "Id",
                textField: "Text",
                url: "/Handler/Area/PublicArea.ashx?Action=AreaJilian&ParentId=0",
                onLoadSuccess: function () {
                    // 这里你需要判断quickCase是否存在值，如果存在，就选中
                    if (requestData && requestData["OneArea"]) {
                        $("#oneArea").combobox("select", requestData["OneArea"]);
                        // 第三步：然后再根据选中的一级的值，去加载二级【已完成】
                        delete requestData["OneArea"];
                        
                    }
                    else { $("#oneArea").combobox("setText", "请选择"); }
                },
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
                onLoadSuccess: function () {
                    // 这里你需要判断quickCase是否存在值，如果存在，就选中
                    if (requestData && requestData["TwoArea"]) {
                        $("#twoArea").combobox("select", requestData["TwoArea"]);
                        delete requestData["TwoArea"];
                    }
                    else { $("#twoArea").combobox("setText", "请选择"); }
                },
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
                url: "/Handler/Area/PublicArea.ashx?Action=AreaJilian&ParentId=" + parentId,
                onLoadSuccess:function(){
                    if(requestData && requestData["ThreeArea"]){
                        $("#threeArea").combobox("select",requestData["ThreeArea"]);
                        delete requestData["ThreeArea"];
                    }
                    else { $("#threeArea").combobox("setText", "请选择"); }
                }
            });
        }
        $(document).ready(function(){
            oneArea();
            $("#twoArea").combobox({
                width: "120px",
            })
            $("#threeArea").combobox({
                width: "120px",
            })
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
                    $("#txbState").combobox("select", requestData["Enable"]);
                }
            });
            // 初始化Form给表单赋值
            $("#UpdateUserForm").form("load", "/Handler/PublicUser.ashx?action=GetUserInfo&Id=" + requestData["Id"] + "&r=" + Math.random());

        });
    </script>
</head>
<body style="margin:0px;padding:0px;">
    <form id="UpdateUserForm" method="post" action="/Handler/PublicUser.ashx">
        <div style="padding: 5px;overflow:hidden">
        <div style="height:20px;padding-left:40px;"><a style="color:red;font-size:14px;font-weight:bold;">*为必填项,用户名和密码不允许修改</a></div>
        <div style="height:20px;"></div>
        <div style="height:35px;">
            <span style="width:100px;height:30px;line-height:30px;text-align:right;display:block;float:left;">区域：</span>
            <input class="easyui-combobox" id="oneArea" name="OneArea" />&nbsp;
            <input class="easyui-combobox" id="twoArea" name="TwoArea"  />&nbsp;
            <input class="easyui-combobox" id="threeArea" name="ThreeArea"  />
        </div>
        <div style="height:35px;">
            <span style="width:100px;height:30px;line-height:30px;text-align:right;display:block;float:left;">用户名：</span>
            <input type="text" class="easyui-textbox" id="txbUserName" name="UserName" style="width: 160px;" disabled="disabled" data-options="required: true" /><a style="color:red;">*</a>
        </div>
        <div style="height:35px;">
            <span style="width:100px;height:30px;line-height:30px;text-align:right;display:block;float:left;">密码：</span>
            <input type="password" class="easyui-textbox" id="txbpassword" name="Password" style="width: 160px;" disabled="disabled" data-options="required: true" /><a style="color:red;">*</a><br />
        </div>
        <div style="height:35px;">
            <span style="width:100px;height:30px;line-height:30px;text-align:right;display:block;float:left;">姓名：</span>
            <input type="text" class="easyui-textbox" id="txbName" name="Name" style="width: 160px;" data-options="required: true" /><a style="color:red;">*</a><br />
        </div>
        <div style="height:35px;">
            <span style="width:100px;height:30px;line-height:30px;text-align:right;display:block;float:left;">联系电话：</span>
            <input type="text" class="easyui-textbox" id="txbPhone" style="width: 160px;" name="Phone" /><br />
        </div>
        <div style="height:35px;">
            <span style="width:100px;height:30px;line-height:30px;text-align:right;display:block;float:left;">手机：</span>
            <input type="text" class="easyui-textbox" id="txbMobile" style="width: 160px;" name="Mobile" /><br />
        </div>
            <div style="height:35px;">
                <span style="width:100px;height:30px;line-height:30px;text-align:right;display:block;float:left;">邮箱：</span>
                <input type="text" class="easyui-textbox" name="Email" id="txbEmail" data-options="validType:'email'" style="width: 160px;" /><br />
            </div>
            <div style="height:35px;">
                <span style="width:100px;height:30px;line-height:30px;text-align:right;display:block;float:left;">地址：</span>
                <input type="text" class="easyui-textbox" name="Address" id="txbAddress" style="width: 160px;" /><br />
            </div>
            <div style="height:35px;">
                <span style="width:100px;height:30px;line-height:30px;text-align:right;display:block;float:left;">状态：</span>
                <input type="text" class="easyui-combobox" name="Enable" id="txbState" style="width: 160px;" /><br />
            </div>
        
            <div style="text-align: left; padding: 10px;">
                <a id="linkConfimation" class="linkbutton">修改</a>
            </div>
        </div>
    </form>

    <script type="text/javascript">

        // 窗口 - 确认按钮
        $("#linkConfimation").linkbutton({
            iconCls: "icon-ok",
            onClick: function () {
                submitUserForm();
            }
        });


    </script>
</body>
</html>
