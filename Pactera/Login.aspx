<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Pactera.Login" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <link href="Css/alogin.css" rel="stylesheet" />
    <link href="Scripts/jquery-easyui-1.4.1/themes/default/signin-pactera.css" rel="stylesheet" />
    <script src="Scripts/jquery-easyui-1.4.1/jquery.min.js"></script>
    <script>
        function resetForm() {
            document.getElementById('UserName').value = '';
            document.getElementById('UserPwd').value = '';
        }
        function onFocus(iput) {
            $(iput).addClass('textbox-focused');
        }
        // 
        function onBlur(iput) {
            $(iput).removeClass('textbox-focused');
        }
    </script>
    <title>用户登录</title>
</head>
<body>
    <form runat="server">
        <div class="Main">
            <div class="left"><img src="images/index_Left.jpg" alt="" width="330" height="330" /></div>
            <div class="right">
                <div style="height: 15px;"></div>
                <div class="title"><img src="images/rig_title.png" width="250" /></div>
                <div style="height:40px;"></div>
                    <div style="width:240px;height:230px;margin-left:30px;">
                    <div class="user-name-box">
                        <input type="text" class="icon-man textbox" onfocus="onFocus(this)" onblur="onBlur(this)" id="UserName" runat="server" />
                    </div>
                    <div style="height: 25px;"></div>
                    <div class="password-box">
                        <input type="password" class="icon-lock textbox" onfocus="onFocus(this)" onblur="onBlur(this)" id="UserPwd" runat="server" />
                    </div>
                    <div style="height: 37px;"></div>
                    <div>
                        <asp:Button ID="btnSignin" class="l-btn" Text="登录" runat="server" OnClick="btnSignin_Click" />
                        <asp:Button ID="btnReset" class="l-btn" Text="重置" OnClientClick="resetForm()" runat="server" />
                    </div>
                    <div class="message">
                        <label id="Message" runat="server"></label>
                    </div>
                </div>
            </div>
        </div>

    </form>
    <div style="text-align:center"><h1>重要通知</h1>
         <p>因服务器在S楼2楼机房运行不稳定，所以决定于2017年1月10日下午将服务器迁移至机房</p>
                    <p>OA登陆网址为：<a href="http://10.128.3.129">10.128.3.129</a>，知识库登陆网址为：
                        <a href="http://10.128.3.129:8002">10.128.3.129:8002</a></p>
                    <p>本服务器将于近期关闭，请互相转告，谢谢!</p>
    </div>
</body>
</html>
