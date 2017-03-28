<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MarketOpportunity.aspx.cs" Inherits="Pactera.MarketInfo.MarketOpportunity" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>市场机会管理</title>
    <link href="../Scripts/My97DatePicker/skin/WdatePicker.css" rel="stylesheet" />
    <script src="../Scripts/My97DatePicker/WdatePicker.js"></script>
    <script src="../Scripts/jquery-easyui-1.4.1/jquery.min.js" charset="utf-8"></script>
    <script src="../Scripts/jquery-easyui-1.4.1/jquery.easyui.min.js" charset="utf-8"></script>
    <link href="../Scripts/jquery-easyui-1.4.1/themes/default/easyui-pactera.css" rel="stylesheet" />
    <link href="../Scripts/jquery-easyui-1.4.1/themes/icon.css" rel="stylesheet" />
    <script src="../Scripts/jquery-easyui-1.4.1/locale/easyui-lang-zh_CN.js" charset="utf-8"></script>
    <link href="../Scripts/jquery-easyui-1.4.1/themes/default/pactera-workflow-description.css" rel="stylesheet" />
    <script src="../Scripts/public.js"></script>
    <%--列表省略号js--%>
    <link href="/Scripts/jquery-easyui-1.4.1/themes/diy_style.css" rel="stylesheet" />
    <style>
    /*显示日期的textbox的样式--灰色*/
    .kgGary{
	    border:#d9d9d9 1px solid;
        border-radius:3px 3px 3px 3px;
	    height:17px;
        font-size:12px; padding-left:4px; padding-top:2px;color:gray;
    }
    </style>
    <script charset="utf-8">
        //将form表单元素值序列化成对象--由于IE下放在公共js中一直不起作用，暂时放在页面
        function serializeObject(form) {
            //alert("开始序列化");
            var o = {};
            $.each(form.serializeArray(), function (index) {
                if (o[this['name']]) {
                    o[this['name']] = o[this['name']] + "," + this['value'];
                    //alert("a:"+this['value']);
                } else {
                    o[this['name']] = this['value'];
                    //alert("b:"+this['value']);
                }
            });
            //alert("序列化结束");
            return o;
        }

        //页面初始化
        $(function () {
            //1.选择登记人
            $('#CreateUser').combobox({
                url: '/Handler/BPM_User.ashx?action=auto_complete',
                mode: 'remote',
                method: 'post',
                textField: 'Value',
                valueField: 'Key',
                panelHeight: 'auto',
                panelMinHeight: 20,
                panelMaxHeight: 200,
                required: true,//验证非空
                novalidate: true,//提交表单时在验证
                onSelect: function (record) {
                    //$('#CreateUserId').val(record.Key);
                    //alert("用户id：" + $('#CreateUserId').val());
                }
            });

            //2.查找时帅选省市区
            $('#comsheng').combobox({
                url: '/Handler/PublicDict.ashx?action=queryarea&AreaType=Province',
                valueField: 'Key',
                textField: 'Value',
                panelHeight: 'auto',
                panelMinHeight: 20,
                panelMaxHeight: 200,
                editable: false,
                onSelect: function (record) {
                    //1).市重新加载
                    $('#comshi').combobox('clear');
                    var url = '/Handler/PublicDict.ashx?action=queryarea&AreaType=City&Parent=' + record.Key;
                    $('#comshi').combobox('reload', url);
                    //2).区重新加载
                    $('#comv').combobox('clear');
                    var url = '/Handler/PublicDict.ashx?action=queryarea&AreaType=District&Parent=0';
                    $('#comv').combobox('reload', url);
                    //3).省-全部：区清空
                    //if (record.Key == '0') {  
                    //    $('#comv').combobox('clear');
                    //    var url = '/Handler/PublicDict.ashx?action=queryarea&AreaType=District&Parent=0';
                    //    $('#comv').combobox('reload', url);
                    //}
                }
            });
            $('#comshi').combobox({
                valueField: 'Key',
                textField: 'Value',
                panelHeight: 'auto',
                panelMinHeight: 20,
                panelMaxHeight: 200,
                editable: false,
                onSelect: function (record) {
                    $('#comv').combobox('clear');
                    var url = '/Handler/PublicDict.ashx?action=queryarea&AreaType=District&Parent=' + record.Key;
                    $('#comv').combobox('reload', url);
                }
            });
            $('#comv').combobox({
                valueField: 'Key',
                textField: 'Value',
                panelHeight: 'auto',
                panelMinHeight: 20,
                panelMaxHeight: 200,
                editable: false
            });

            //3.gsjc-公司简称-查询
            $('#gsjc').combobox({
                url: '/Handler/PublicBPM_Corp.ashx?action=info1',
                //mode: 'remote',
                //method: 'post',
                panelHeight: 'auto',
                panelMinHeight: 20,
                panelMaxHeight: 200,
                editable: false,
                valueField: 'PK_CORP',
                textField: 'UNITSHORTNAME',
                onSelect: function (record) {
                    if (record.PK_CORP) {
                        $('#gsjc1').val(record.PK_CORP);
                    }
                }
            });

            //7.开始时间变化时，截至时间选择范围变化--现在用WdatePicker日期控件，不用这里限制截至时间的最小日期
            //$('#begintime1').datebox({
            //    onSelect: function (date) {
            //        var stime = $('#begintime1').datebox('getValue');
            //        stime = stime.replace("-", "/");//替换字符，变成标准格式    
            //        var stime1 = new Date(Date.parse(stime));
            //        //alert("begin:" + stime);

            //        $('#endtime1').datebox().datebox('calendar').calendar({
            //            validator: function (date) {
            //                var now = new Date();
            //                var d1 = new Date(now.getFullYear(), now.getMonth(), now.getDate());
            //                var d2 = new Date(now.getFullYear(), now.getMonth(), now.getDate() + 10);
            //                return stime1 <= date;
            //            }
            //        });
            //    }
            //});

            //4.加载列表table
            $('#tblist').datagrid({
                url: '/Handler/MaketOperate.ashx?flag=select',
                //toolbar: '#tooldiv',
                idField: 'Id',  //主键
                pagination: true, //是否分页-yes
                nowrap: true,  //是否换行-no
                fitcolumns: true, //列自动展开/收缩到合适的DataGrid宽度。
                singleSelect: true, //是否单选一行-yes
                width: '100%',
                rownumbers: true,
                height: 315,
                columns: [[
                {  //halign:设置表头对齐方式，align：设置列对齐方式，默认left
                    field: 'CreateCorp1', title: '公司简称', width: '9%', halign: 'center', sortable: true,
                    formatter: function (value, row, index) {
                        return '<span title="' + value + '">' + value + '</span>';  //value用""引起来，在value包含空格时，能提示全部内容，否则就提示空格前的部分内容
                    }
                 },
                 //{
                 //    field: 'aa', title: 'ID', width: '6%', halign: 'center', sortable: true,
                 //    formatter: function (value, row, index) {
                 //        return '<span title=' + value + '>' + value + '</span>';
                 //    }
                 //},
                {
                    field: 'RegNo', title: '登记编号', width: '6%', halign: 'center', sortable: true,
                    formatter: function (value, row, index) {
                        return '<span title="' + value + '">' + value + '</span>';
                    }
                },
                {
                    field: 'BuildCopeName', title: '建设单位', width: '10%', halign: 'center', sortable: true,
                    formatter: function (value, row, index) {
                        return '<span title="' + value + '">' + value + '</span>';
                    }
                },
                {
                    field: 'ProjectName',
                    title: '工程名称', width: '14%', halign: 'center', sortable: true,
                    formatter: function (value, row, index) {
                        return '<span title="' + value + '">' + value + '</span>';
                    }
                },
                {
                    field: 'ProvinceName',
                    title: '所属省市', width: '5%', halign: 'center', align: 'center',
                    formatter: function (value, row, index) {
                        return '<span title="' + value + '">' + value + '</span>';
                    }
                }, {
                    field: 'CityName',
                    title: '所属地市', width: '6%', halign: 'center', align: 'center',
                    formatter: function (value, row, index) {
                        return '<span title="' + value + '">' + value + '</span>';
                    }
                }, {
                    field: 'DistrictName',
                    title: '所属区县', width: '6%', halign: 'center', align: 'center',
                    formatter: function (value, row, index) {
                        return '<span title="' + value + '">' + value + '</span>';
                    }
                },
                {
                    field: 'CreateTime',
                    title: '登记时间', width: '7%', halign: 'center', sortable: true, align: 'center',
                    formatter: function (value, row, index) {
                        return '<span title="' + value + '">' + value + '</span>';
                    }
                },
                {
                    field: 'BuildArea',
                    title: '建筑面积(m²)', width: '7%', halign: 'center', sortable: true, align: 'right',
                    formatter: function (value, row, index) {
                        return '<span title="' + value + '">' + value + '</span>';
                    }
                }, {
                    field: 'BuildCost',
                    title: '造价(万元)', width: '6%', halign: 'right', sortable: true, align: 'right',
                    formatter: function (value, row, index) {
                        return '<span title="' + value + '">' + value + '</span>';
                    }
                }, {
                    field: 'UserName',
                    title: '登记人', width: '5%', halign: 'center', sortable: true, align: 'center',
                    formatter: function (value, row, index) {
                        return '<span title="' + value + '">' + value + '</span>';
                    }
                }, {
                    field: 'ApprovalStatus',
                    title: '审批状态',
                    width: '6%',
                    halign: 'center',
                    sortable: true,
                    align: 'left',
                    formatter: function (value, row, index) {
                        if (value == '-1') {
                            return '未提交';
                        } else if (value == '4') {
                            return '审批通过';
                        } else if (value == '5') {
                            return '审批未通过';
                        } else {
                            return '审批中';
                        }
                    }
                },
                {
                    field: 'CoordinationResult1',
                    title: '协调结果', width: '5%', halign: 'center', sortable: true, align: 'center',
                    formatter: function (value, row, index) {
                        return '<span title="' + value + '">' + value + '</span>';
                    }
                },
                {
                    field: 'CoordinationMessage',
                    title: '批示意见', width: '6%', halign: 'center', sortable: true, align: 'center',
                    formatter: function (value, row, index) {
                        return '<span title="' + value + '">' + value + '</span>';
                        //return '<a  title="' + value + '">'+ value + '</a>';  //<span>可以用<a>替换
                    }
                },
                {
                    field: 'CreateDate', //保存主键Id(注意：如果该列出现多次的话，hidden: true，隐藏不起作用)
                    title: '时间',
                    hidden: true
                }
                ]],
                //表格加载成功
                onLoadSuccess: function (data) {
                    ////1、左侧分页table
                    //var table = $("#dataPagerButtonSpace").parent().parent().parent().parent();
                    //var options = $('#tblist').datagrid('getPager').data("pagination").options;
                    //var total = options.total;
                    //var pagecount = Math.ceil(total / options.pageSize);
                    //$(table).find("td").each(function (index) {
                    //    if (index == 7) {
                    //        $(this).html('页&nbsp;&nbsp;共 ' + pagecount + ' 页，共 ' + total + ' 条&nbsp;&nbsp;');
                    //    }
                    //});
                },
                //点击行，根据选中行的审批状态，设置修改和删除按钮是否可用
                onClickRow: function (rowIndex, rowData) {
                    //$('#btn_edit').linkbutton('enable');
                    //$('#btn_remove').linkbutton('enable');
                    //alert("rowData.ApprovalStatus:" + rowData.ApprovalStatus + "///rowData.CoordinationResult1:" + rowData.CoordinationResult1);
                    //alert("rowData.ApprovalStatus:" + rowData.ApprovalStatus);
                    if (rowData.ApprovalStatus == '-1' ) {
                        $('#btn_remove').linkbutton('enable');
                        $('#btn_edit').linkbutton('enable');
                    }
                    else if (rowData.ApprovalStatus == '5') {
                        $('#btn_remove').linkbutton('enable');
                        if(rowData.CoordinationResult1 == '')
                            $('#btn_edit').linkbutton('enable');
                        //else
                        //    $('#btn_edit').linkbutton('disable'); //禁用
                    }
                    //if (rowData.ApprovalStatus == '-1' || rowData.ApprovalStatus == '5') {
                    //    $('#btn_remove').linkbutton('enable');
                    //}
                    //else if (rowData.ApprovalStatus == '-1' || (rowData.ApprovalStatus == '5' && rowData.CoordinationResult1 == '')) {
                    //    $('#btn_edit').linkbutton('enable');  //启用--未提交的/退回并且协调结果为''
                    //} 
                    else {
                        $('#btn_edit').linkbutton('disable'); //禁用
                        $('#btn_remove').linkbutton('disable');
                        //alert("为提交/退回,rowData.CreateUserId:"+rowData.CreateUserId);
                        //获取当前用户
                        //if (rowData.CreateUserId != '') {  //有登记人是验证当前人是否是登记人
                        //    $.ajax({
                        //        type: 'post',
                        //        url: '/Handler/MaketOperate.ashx',
                        //        data: { flag: 'getcurruser', time: new Date().getTime() },
                        //        dataType: 'text',
                        //        success: function (uid) {
                        //            if (uid) {
                        //                //alert("当前：" + uid + "///rowData.CreateUserId:" + rowData.CreateUserId);
                        //                if (uid == rowData.CreateUserId) {
                        //                    $('#btn_edit').linkbutton('enable');  //启用
                        //                    $('#btn_remove').linkbutton('enable');
                        //                }
                        //                else {
                        //                    $('#btn_edit').linkbutton('disable'); //禁用
                        //                    $('#btn_remove').linkbutton('disable');
                        //                    //alert("不是当前人");
                        //                }
                        //            }
                        //        }
                        //    });
                        //}
                        //else {  //无登记人时，启用
                        //    $('#btn_edit').linkbutton('enable');  //启用
                        //    $('#btn_remove').linkbutton('enable');
                        //}
                    }
                },
                //双击，显示详细
                onDblClickRow: function (rowIndex, rowData) {
                    var url = '/MarketInfo/DetailInfo.aspx?action=xtdetail&keys=' + rowData.Id + '&time=' + new Date().getTime();
                    parent.createAndOpenModalWindow('查看工程信息', 950, 500, url, function (data) {

                    });
                }
            });
            //5.Grid分页
            //$.fn.pagination.defaults.displayMsg = '';  //显示数据条数：不显示
            //var pager = $('#tblist').datagrid().datagrid('getPager');	// get the pager of datagrid
            //设置分页控件
            var pager = $("#tblist").datagrid("getPager");
            $(pager).pagination({
                beforePageText: "第",//页数文本框前显示的汉字 
                afterPageText: "页 共 {pages} 页",
                displayMsg: "当前显示 {from} - {to} 条记录   共 {total} 条记录"
            });

            //6.给Grid添加按钮
            AddGridBtn(pager);
        });

        //表格命令按钮
        function AddGridBtn(pager) {
            pager.pagination({
                buttons: [{
                    //id: 'dataPagerButtonSpace'
                }, {
                    id: 'btn_add',
                    iconCls: 'icon-add',
                    text: '添加',
                    handler: function () {
                        Action.Add();
                    }
                }, {
                    id: 'btn_edit',
                    iconCls: 'icon-edit',
                    text: '编辑',
                    handler: function () {
                        Action.Update();
                    }
                }, {
                    id: 'btn_remove',
                    iconCls: 'icon-remove',
                    text: '删除',
                    handler: function () {
                        Action.Delete();
                    }
                }, {
                    id: 'btn_info',
                    iconCls: 'icon-info',
                    text: '详细信息',
                    handler: function () {
                        Action.Detail();
                    }
                }, {
                    id: 'btn_export',
                    iconCls: 'icon-export',
                    text: '导出',
                    handler: function () {
                        ExportInfo();
                    }
                }]
            });
            
            //$(".datagrid-header-rownumber").text("序号");
            //$("#dataPagerButtonSpace").removeAttr("class").removeAttr("href").removeAttr("group");
            //var table = $("#dataPagerButtonSpace").parent().parent().parent().parent();
            //var datagridPager = $("#dataPagerButtonSpace").parent().parent().parent().parent().parent();
            //$(datagridPager).css("position", "relative");
            //var newTable = $(table).clone().css("position", "absolute").css("top", "0px").css("right", "10px");
            //// 移除旧TD
            //$(table).find("td").each(function (index) {
            //    if (index > 14) $(this).remove();
            //});
            //// 移除多余的分页TD
            //$(newTable).find("td").each(function (index) {
            //    if (index <= 14) $(this).remove();
            //});
            //datagridPager.append(newTable);
            //// 这里添加事件，需要重新添加，克隆后对象的属性关联不过来
            //$("#btn_add").click(function () {
            //    Action.Add();
            //});
            //$("#btn_edit").click(function () {
            //    if ($('#hdused').val() == "1") {
            //        Action.Update();
            //    }
            //});
            //$("#btn_remove").click(function () {
            //    if ($('#hdused').val() == "1") {
            //        Action.Delete();
            //    }
            //});
            //$("#btn_info").click(function () {
            //    Action.Detail();
            //});
            //$("#btn_export").click(function () {
            //    ExportInfo();
            //});
            //$('.datagrid-header-rownumber').text('序号');
        }

        //导出
        function ExportInfo() {
            $('#serMarkfm').submit();
        }

        //编辑操作
        var Action = {
            //1.查询
            Search: function () {
                //alert("Search");
                //1、验证开始结束时间是否都输入
                var st = $('#begintime1').val();
                var et = $('#endtime1').val();
                st = $.trim(st);
                et = $.trim(et);
                //alert("st:"+st+"///et:"+et);
                if ((st != '' && et == '') || (st == '' && et != '')) {
                    parent.$.messager.alert('提示', '请输入开始和截止时间！', 'info');
                    return;
                }
                //2、验证批示开始结束时间是否都输入
                st = $('#psbegintime').val();
                et = $('#psendtime').val();
                st = $.trim(st);
                et = $.trim(et);
                //alert("st:"+st+"///et:"+et);
                if ((st != '' && et == '') || (st == '' && et != '')) {
                    parent.$.messager.alert('提示', '请输入批示开始和截止时间！', 'info');
                    return;
                }

                $('#tblist').datagrid('load', serializeObject($('#serMarkfm')));
                
            },

            //2.添加按钮
            Add: function () {
                var url='/MarketInfo/AddMarket.aspx?action=add&time=' + new Date().getTime();
                parent.createAndOpenModalWindow('添加工程信息', 950, 500, url, function (data) {
                    $('#tblist').datagrid('reload');  //重新加载列表
                });
            },

            //3.修改按钮
            Update: function () {
                //1、选中行
                var row = $('#tblist').datagrid('getSelected');
                if (row == null || row.length == 0) {
                    parent.$.messager.alert('提示', '请选择要编辑的数据！', 'info');
                    return;
                }

                //判断审批状态
                if (row.ApprovalStatus != '-1' && row.ApprovalStatus != '5') {
                    parent.$.messager.alert('提示', '状态是“未提交”、“审批未通过”的数据才可编辑！', 'info');
                    return;
                }
                
                var url = '/MarketInfo/AddMarket.aspx?action=update&keys=' + row.Id + '&state=' + row.ApprovalStatus + '&uid='+row.CreateUserId+'&time=' + new Date().getTime();
                parent.createAndOpenModalWindow('修改工程信息', 950, 500, url, function (data) {
                    $('#tblist').datagrid('reload');  //重新加载列表
                });
            },

            //4.删除
            Delete: function () {
                //1).获取选中行
                //getSelected：  获取所选择行的第一行数据，如果没有行被选择返回null，否则返回数据记录
                //getSelections：获取所有选择行数据，返回数组数据，里面的数组元素就是数据记录。
                var row = $('#tblist').datagrid('getSelected');

                //2).询问是否确定删除
                if (row == null || row.length == 0) {
                    parent.$.messager.alert('提示', '请选择要删除的数据！', 'info');
                    return;
                } else {
                    //判断审批状态
                    if (row.ApprovalStatus != '-1' && row.ApprovalStatus != '5') {
                        parent.$.messager.alert('提示', '状态是“未提交”、“审批未通过”的数据才可删除！', 'info');
                        return;
                    }
                    $.messager.confirm('确认', '确定要删除选择数据？', function (result) {
                        if (!result) {
                            return;
                        } else {
                            //3).记录参数键值信息
                            var keys = row.Id;  //注意：字段名必须和数据库字段名一样，区分大小写
                            //alert("keys:" + keys);

                            //4).调用方法删除
                            $.ajax({
                                type: "post",
                                url: "/Handler/MaketOperate.ashx", //调用控制器的方法，进行删除
                                data: { flag: 'delete', keys: keys, ran: Math.random() },
                                dataType: "text",
                                async: false,  //是否同步
                                success: function (msg) {
                                    Action.Search();
                                    parent.$.messager.alert('提示', '删除成功！', 'info');
                                    return;
                                },
                                error: function () {
                                    parent.$.messager.alert('提示', '删除失败！', 'info');
                                    return;
                                }
                            });
                        }
                    });
                }
                Action.Search();
            },

            //5.查询重置
            ClearSer: function () {
                $('#serMarkfm').form('clear');  //重置表单数据
                $('#tblist').datagrid('sort', {	        // 指定了排序顺序的列--列必须是在列表中出现的字段
                    sortName: 'CreateDate',
                    sortOrder: 'desc'
                });
                $('#tblist').datagrid('load', { flag: 'select' });
                $('#filename').val('市场机会列表.xls');
                $('#sheetname').val('市场机会列表');
            },

            //6.详细按钮
            Detail: function () {
                //1、选中行
                var row = $('#tblist').datagrid('getSelected');
                if (row == null || row.length == 0) {
                    parent.$.messager.alert('提示', '请选择要查看的数据！', 'info');
                    return;
                }
                
                var url = '/MarketInfo/DetailInfo.aspx?action=xtdetail&keys=' + row.Id + '&time=' + new Date().getTime();
                parent.createAndOpenModalWindow('查看工程信息', 950, 500, url, function (data) {
                    
                });
            },

            //10.根据枚举value，获取pvalue,使得施工领域默认选中获取pvalue，然后再根据pvalue，加载工程类型选项，默认选择id项
            GetPid: function (id) {
                $.ajax({
                    type: 'post',
                    url: '/Handler/PublicEnum.ashx?action=enum_getPid&value=' + id,
                    dataType: 'text',
                    success: function (data) {
                        $('#ProjectType').combobox('setValue', ''); //工程类型
                        $('#ConstructionDoman').combobox('setValue', '');
                        $('#ProjectType').combobox('disableValidation');  //不验证是否必填
                        $('#ConstructionDoman').combobox('disableValidation');  

                        if (data && data.length == 36) {
                            var data1 = $('#ConstructionDoman').combobox('getData');
                            if (data1.length > 0) {
                                for (var i = 0; i < data1.length; i++) {
                                    if (data == data1[i].Key) {  //data是有效的
                                        $('#ConstructionDoman').combobox('select', data);  //施工领域

                                        //工程类型，根据所选的施工领域，重新加载
                                        $('#ProjectType').combobox('clear');
                                        var url = '/Handler/PublicEnum.ashx?action=enum_auto&pvalue=' + data;  //escape(name) :转换中文参数
                                        $('#ProjectType').combobox('reload', url);
                                        $('#ProjectType').combobox('select', id); //工程类型
                                        break;
                                    }
                                }
                            }
                        }
                    }
                });
            },

            //11.根据枚举value，获取pvalue,使得使用资质默认选中获取pvalue，然后再根据pvalue，加载上班公司选项，默认选择id项
            GetPid1: function (id) {
                $.ajax({
                    type: 'post',
                    url: '/Handler/PublicEnum.ashx?action=enum_getPid&value=' + id,
                    dataType: 'text',
                    success: function (data) {
                        $('#UseQualifications').combobox('setValue', ''); //使用资质
                        $('#ReportCorp').combobox('setValue', '');  //上报公司
                        $('#UseQualifications').combobox('disableValidation');  //不验证是否必填
                        $('#ReportCorp').combobox('disableValidation');

                        if (data && data.length == 36) {
                            var data1 = $('#UseQualifications').combobox('getData');
                            if (data1.length > 0) {
                                for (var i = 0; i < data1.length; i++) {
                                    if (data == data1[i].Key) {  //data是有效的
                                        $('#UseQualifications').combobox('select', data);  //施工领域

                                        //上报公司，根据所选的使用资质，重新加载
                                        $('#ReportCorp').combobox('clear');
                                        var url = '/Handler/PublicEnum.ashx?action=enum_auto&pvalue=' + data;  //escape(name) :转换中文参数
                                        $('#ReportCorp').combobox('reload', url);
                                        $('#ReportCorp').combobox('select', id); //上报公司
                                        break;
                                    }
                                }
                            }
                        }
                    }
                });
            }
        }
    </script>
</head>
<body>
    <div style="margin: 8px 0px 0px 0px;">
        <div class="toubu">
            <p>查询条件</p>
        </div>
        <div class="easyui-panel" style="width: 100%; background-color: #fff;">
            <form id="serMarkfm" method="post" action="../Handler/ExportExcel.aspx?action=ProjectChance">
                <input type="hidden" id="filename" name="filename" value="市场机会列表.xls" />
                <input type="hidden" id="sheetname" name="sheetname" value="市场机会列表" />
                <table cellpadding="3px" style="margin-left: 10px;">
                    <tr>
                        <td>公司简称:</td>
                        <td>
                            <input name="gsjc" id="gsjc" class="easyui-textbox" style="width: 180px;" />
                            <input name="gsjc1" id="gsjc1" type="hidden" />
                        </td>
                        <td style="width: 25px"></td>
                        <td>登记编号:</td>
                        <td>
                            <input id="djbh1" name="djbh1" class="easyui-textbox" data-options="validType:['intx','length[1,50]']" style="width: 180px;" /></td>
                        <td style="width: 25px"></td>
                        <td>建设单位:</td>
                        <td>
                            <input id="jsdw1" name="jsdw1" class="easyui-textbox" data-options="validType:'length[1,100]'" style="width: 180px;" /></td>
                        <td style="width: 25px"></td>
                        <td>工程名称:</td>
                        <td>
                            <input id="gcmc1" name="gcmc1" class="easyui-textbox" data-options="validType:'length[1,150]'" style="width: 180px;" /></td>
                    </tr>
                    <tr>
                        <td>所属省市:</td>
                        <td>
                            <input class="easyui-textbox" type="text" name="comsheng" id="comsheng" style="width: 180px;" />
                        </td>
                        <td></td>
                        <td>所属地市:</td>
                        <td>
                            <input class="easyui-textbox" type="text" name="comshi" id="comshi" style="width: 180px;" />
                        </td>
                        <td></td>
                        <td>所属区县:</td>
                        <td>
                            <input class="easyui-textbox" type="text" name="comv" id="comv" style="width: 180px;" />
                        </td>
                        <td></td>
                        <td>工程地点:</td>
                        <td>
                            <input id="gcdd1" name="gcdd1" class="easyui-textbox" data-options="validType:'length[1,150]'" style="width: 180px;" /></td>
                    </tr>
                    <tr>
                        <td>登记起日:</td>
                        <td>
                            <input id="begintime1" name="begintime1" type="text" class="Wdate" onclick="WdatePicker();" maxlength="10" style="width: 173px;" />
                        </td>
                        <td></td>
                        <td>登记至日:</td>
                        <td>
                            <input id="endtime1" name="endtime1" type="text" class="Wdate" onclick="WdatePicker({ minDate: '#F{$dp.$D(\'begintime1\',{d:1})}' });"
                                maxlength="10" style="width: 173px;" />
                        </td>
                        <td></td>
                        <td>登记人员:</td>
                        <td>
                            <select name="CreateUser" id="CreateUser" style="width: 180px;">
                            </select>
                            <input id="CreateUserId" name="CreateUserId" type="hidden" />
                        </td>
                        <td></td>
                        <td>所属区域:</td>
                        <td>
                            <select id="RegionId1" name="RegionId1" class="easyui-combobox" style="width: 180px;"
                                data-options="
                                panelHeight: 'auto',
                                panelMinHeight: 20,
                                panelMaxHeight: 200,
                                editable:false,
                                url: '/Handler/PublicArea.ashx?action=enum_auto_q',
                                valueField: 'Key',
                                textField: 'Value'">
                            </select>
                        </td>
                    </tr>
                    <tr>
                        <td>批示起日:</td>
                        <td>
                            <input id="psbegintime" name="psbegintime" type="text" class="Wdate" onclick="WdatePicker();" maxlength="10" style="width: 173px;" />
                        </td>
                        <td></td>
                        <td>批示至日:</td>
                        <td>
                            <input id="psendtime" name="psendtime" type="text" class="Wdate" onclick="WdatePicker({ minDate: '#F{$dp.$D(\'psbegintime\',{d:1})}' });"
                                maxlength="10" style="width: 173px;" />
                        </td>
                        <td></td>
                        <td>审批状态:</td>
                        <td>
                            <select id="spztdr" name="spztdr" class="easyui-combobox" style="width: 180px;"
                                data-options="
                                editable:false,
                                panelHeight: 'auto',
                                panelMinHeight: 20,
                                panelMaxHeight: 200">
                                <option value="">全部</option>
                                <option value="-1">未提交</option>
                                <option value="1">审批中</option>
                                <option value="4">审批通过</option>
                                <option value="5">审批未通过</option>
                            </select>
                        </td>
                        <td></td>
                        <td></td>
                        <td style="float: right;">
                            <%-- <div class="a1" onclick="Action.Search()"></div>
                            <div class="a2" onclick="Action.ClearSer()"></div>--%>
                            <a class="easyui-linkbutton linkbutton" style="margin-left: 0px;" iconcls="icon-search" onclick="Action.Search()">查询</a>
                            <a class="easyui-linkbutton linkbutton" style="margin-left: 5px;" iconcls="icon-reset" onclick="Action.ClearSer()">重置</a>
                        </td>
                    </tr>
                </table>
            </form>
        </div>
    </div>
    <div style="margin: 0px 0px 0px 0px;">
        <div class="toubu">
            <p>市场列表</p>
        </div>
        <table id="tblist"></table>
        <input type="hidden" name="shaixuan" id="shaixuan" value="0" />
    </div>

    <input type="hidden" id="hdused" value="1" /> <%--标识编辑和删除按钮是否启用--%>
    <div id="mapdiv">
        <%--<iframe id="ifm" style="width:800px; height:366px">

        </iframe>--%>
    </div>
</body>
</html>

