using NPOI.HSSF.UserModel;
using Pactera.Common.DataExport;
using Pactera.Core.Configuration;
using Pactera.Data;
using Pactera.Model;
using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;

namespace Pactera.Handler
{
    /// <summary>
    /// ExportExcel 的摘要说明
    /// </summary>
    public class ExportXml : Pactera.Web.BaseHttpHandler, IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            var dtExport = new DataTable();

            var action = context.Request["Action"] ?? "";
            var sheetname = context.Request["SheetName"] ?? "报表";
            var filename = context.Request["FileName"] ?? "报表.xls";

            // 如果是非火狐浏览器对文件名进行特殊处理（URL编码，IE浏览器不支持汉字文件名，需要对汉字进行编码，否则会乱码）。
            if (context.Request.UserAgent.ToLower().IndexOf("firefox") < 0)
            {
                filename = HttpUtility.UrlEncode(filename, System.Text.Encoding.UTF8);
            }

            // 获取数据
            switch (action)
            {
                case "DaibanCaseList":
                    dtExport = GetCaseInfoTable(context);
                    break;
                case "UnFinished":
                    dtExport = getUnfinishedInfoTable(context);
                    break;
                case "Finished":
                    dtExport = getFinishedInfoTable(context);
                    break;
                case "Customer":
                    dtExport = getCustomerInfoTable(context);
                    break;
                case "CusHistory":
                    dtExport = getCusHistory(context);
                    break;
                case "CaseList":
                    dtExport=getCaseListInfoTable(context);
                    break;
                case "CaseYjing":
                    dtExport = getcaseYujingInfoTable(context);
                    break;
                case "CaseQuickCase":
                    dtExport = getQuickCase(context);
                    break;
                default:
                    break;
            }

            // 生成excel--dt表格有数据时
            if (dtExport != null && dtExport.Rows.Count > 0)
            {
                byte[] data = Pactera.Common.DataExport.ExportXml.DataTableToXmlByte(dtExport, "工单表");

                // 设置头值
                context.Response.ContentType = "text/xml";
                context.Response.Headers.Add("content-disposition:", "attachment; filename=" + filename);

                // 输出文件到客户端
                context.Response.BinaryWrite(data);
                context.Response.End();
            }
            else
            {
                /*
                byte[] data = exc.TableToExcel(new DataTable(), sheetname);

                // 设置头值
                context.Response.ContentType = "application/nvd.ms-excel";
                context.Response.Headers.Add("content-disposition:", "attachment; filename=" + filename);

                // 输出文件到客户端
                context.Response.BinaryWrite(data);
                context.Response.End();*/
            }
        }

        /// <summary>
        /// 待办任务导出报表
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private DataTable GetCaseInfoTable(HttpContext context)
        {
            var dbm = DataBaseFactory.Instance.Create();

            var htCondition = new Hashtable();

            htCondition.Add("Receiver", CurrentSigninUser.Name);
            htCondition.Add("State <> @State", new SqlParameter("@State", "Complete"));

            // ===
            string field = "Id,Receiver,(select Text from Cus_CaseType where Id =(select OneValue from Cus_case where Id = CaseId))AS OneValue,(select Text from Cus_CaseType where Id =(select TwoValue from Cus_case where Id = CaseId))AS TwoValue,(select Text from Cus_CaseType where Id =(select ThreeValue from Cus_case where Id = CaseId))AS ThreeValue,(select Text from Cus_CaseType where Id =(select FourValue from Cus_case where Id = CaseId))AS FourValue,(select Name from Sys_User where Id = (select Created from Cus_case where Id = CaseId)) AS Created,(select Caseleibie from Cus_case where Id=CaseId )AS Caseleibie,(select CaseNo from Cus_case where Id=CaseId) AS CaseNo,(select Casetype from Cus_AjType where Id=(select Ajtype from Cus_case where Id=CaseId)) AS Ajtype,(select CustomerNo from Cus_case where Id=CaseId) AS CustomerNo,(select State from Cus_case where Id=CaseId) AS State,(select Sourcetext from Cus_source where Id = (select Source from Cus_case where Id=CaseId)) AS Source,(select Caseleveltext from Cus_caseLevel where Id = (select Caselevel from Cus_case where Id=CaseId)) AS Caselevel,(select Prioritytext from Cus_priority where Id = (select Priority from Cus_case where Id=CaseId)) AS Priority,(select Stringenttext from Cus_stringent where Id = (select Stringent from Cus_case where Id=CaseId)) AS Stringent,(select Enineer from Cus_case where Id = CaseId) AS Enineer,(select Name from Sys_User where Id = (select handler from Cus_case where Id = CaseId)) AS handler,(select AssignedDate from Cus_case where Id = CaseId)AS AssignedDate";
            string table = "Cus_Workflow";

            var dtCase = dbm.GetDataTable(field, table, htCondition, "Id");

            dtCase.TableName = "Cus_Workflow";
            dtCase.Columns["Id"].Caption = "编号";
            dtCase.Columns["Created"].Caption = "建单人";
            dtCase.Columns["Caseleibie"].Caption = "Case类别";
            dtCase.Columns["CaseNo"].Caption = "Case编号";
            dtCase.Columns["OneValue"].Caption = "一级Case类型";
            dtCase.Columns["TwoValue"].Caption = "二级Case类型";
            dtCase.Columns["ThreeValue"].Caption = "三级Case类型";
            dtCase.Columns["FourValue"].Caption = "四级Case类型";
            dtCase.Columns["Ajtype"].Caption = "案件类型";
            dtCase.Columns["CustomerNo"].Caption = "客户编号";
            dtCase.Columns["State"].Caption = "状态";
            dtCase.Columns["Source"].Caption = "来源";
            dtCase.Columns["Caselevel"].Caption = "故障级别";
            dtCase.Columns["Priority"].Caption = "优先级";
            dtCase.Columns["Stringent"].Caption = "紧急性";
            dtCase.Columns["Enineer"].Caption = "工程师";
            dtCase.Columns["handler"].Caption = "处理人";
            dtCase.Columns["AssignedDate"].Caption = "建单时间";
            dtCase.PrimaryKey = new DataColumn[] { dtCase.Columns["Id"] };

            dtCase.AcceptChanges();

            return dtCase;
        }


        /// <summary>
        /// 已办未完成任务导出报表
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private DataTable getUnfinishedInfoTable(HttpContext context)
        {
            var dbm = DataBaseFactory.Instance.Create();

            var htCondition = new Hashtable();

            htCondition.Add("Receiver <> @Receiver", new SqlParameter("@Receiver", CurrentSigninUser.Name));
            htCondition.Add("State <> @State", new SqlParameter("@State", "Complete"));
            htCondition.Add("handler", CurrentSigninUser.Id);

            // ===
            string field = "Id,Receiver,(select Text from Cus_CaseType where Id =(select OneValue from Cus_case where Id = CaseId))AS OneValue,(select Text from Cus_CaseType where Id =(select TwoValue from Cus_case where Id = CaseId))AS TwoValue,(select Text from Cus_CaseType where Id =(select ThreeValue from Cus_case where Id = CaseId))AS ThreeValue,(select Text from Cus_CaseType where Id =(select FourValue from Cus_case where Id = CaseId))AS FourValue,(select Name from Sys_User where Id = (select Created from Cus_case where Id = CaseId)) AS Created,(select Caseleibie from Cus_case where Id=CaseId )AS Caseleibie,(select CaseNo from Cus_case where Id=CaseId) AS CaseNo,(select Casetype from Cus_AjType where Id=(select Ajtype from Cus_case where Id=CaseId)) AS Ajtype,(select CustomerNo from Cus_case where Id=CaseId) AS CustomerNo,(select State from Cus_case where Id=CaseId) AS State,(select Sourcetext from Cus_source where Id = (select Source from Cus_case where Id=CaseId)) AS Source,(select Caseleveltext from Cus_caseLevel where Id = (select Caselevel from Cus_case where Id=CaseId)) AS Caselevel,(select Prioritytext from Cus_priority where Id = (select Priority from Cus_case where Id=CaseId)) AS Priority,(select Stringenttext from Cus_stringent where Id = (select Stringent from Cus_case where Id=CaseId)) AS Stringent,(select Enineer from Cus_case where Id = CaseId) AS Enineer,(select Name from Sys_User where Id = (select handler from Cus_case where Id = CaseId)) AS handler,(select AssignedDate from Cus_case where Id = CaseId)AS AssignedDate";
            string table = "Cus_Workflow";

            var dtCase = dbm.GetDataTable(field, table, htCondition, "Id");

            dtCase.TableName = "Cus_Workflow";
            dtCase.Columns["Id"].Caption = "编号";
            dtCase.Columns["Created"].Caption = "建单人";
            dtCase.Columns["Caseleibie"].Caption = "Case类别";
            dtCase.Columns["CaseNo"].Caption = "Case编号";
            dtCase.Columns["OneValue"].Caption = "一级Case类型";
            dtCase.Columns["TwoValue"].Caption = "二级Case类型";
            dtCase.Columns["ThreeValue"].Caption = "三级Case类型";
            dtCase.Columns["FourValue"].Caption = "四级Case类型";
            dtCase.Columns["Ajtype"].Caption = "案件类型";
            dtCase.Columns["CustomerNo"].Caption = "客户编号";
            dtCase.Columns["State"].Caption = "状态";
            dtCase.Columns["Source"].Caption = "来源";
            dtCase.Columns["Caselevel"].Caption = "故障级别";
            dtCase.Columns["Priority"].Caption = "优先级";
            dtCase.Columns["Stringent"].Caption = "紧急性";
            dtCase.Columns["Enineer"].Caption = "工程师";
            dtCase.Columns["handler"].Caption = "处理人";
            dtCase.Columns["AssignedDate"].Caption = "建单时间";
            dtCase.PrimaryKey = new DataColumn[] { dtCase.Columns["Id"] };

            dtCase.AcceptChanges();

            return dtCase;
        }
            
        /// <summary>
        /// 已办已完成任务导出报表
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private DataTable getFinishedInfoTable(HttpContext context)
        {
            var dbm = DataBaseFactory.Instance.Create();

            var htCondition = new Hashtable();

            htCondition.Add("handler", CurrentSigninUser.Id);

            if (dbm.Exists("Cus_WorkflowLog", htCondition))
            {
                htCondition.Clear();
                htCondition.Add("State", "Complete");
                htCondition.Add("handler", CurrentSigninUser.Id);
            }

            // ===
            string field = "Id,Receiver,(select Text from Cus_CaseType where Id =(select OneValue from Cus_case where Id = CaseId))AS OneValue,(select Text from Cus_CaseType where Id =(select TwoValue from Cus_case where Id = CaseId))AS TwoValue,(select Text from Cus_CaseType where Id =(select ThreeValue from Cus_case where Id = CaseId))AS ThreeValue,(select Text from Cus_CaseType where Id =(select FourValue from Cus_case where Id = CaseId))AS FourValue,(select Name from Sys_User where Id = (select Created from Cus_case where Id = CaseId)) AS Created,(select Caseleibie from Cus_case where Id=CaseId )AS Caseleibie,(select CaseNo from Cus_case where Id=CaseId) AS CaseNo,(select Casetype from Cus_AjType where Id=(select Ajtype from Cus_case where Id=CaseId)) AS Ajtype,(select CustomerNo from Cus_case where Id=CaseId) AS CustomerNo,(select State from Cus_case where Id=CaseId) AS State,(select Sourcetext from Cus_source where Id = (select Source from Cus_case where Id=CaseId)) AS Source,(select Caseleveltext from Cus_caseLevel where Id = (select Caselevel from Cus_case where Id=CaseId)) AS Caselevel,(select Prioritytext from Cus_priority where Id = (select Priority from Cus_case where Id=CaseId)) AS Priority,(select Stringenttext from Cus_stringent where Id = (select Stringent from Cus_case where Id=CaseId)) AS Stringent,(select Enineer from Cus_case where Id = CaseId) AS Enineer,(select Name from Sys_User where Id = (select handler from Cus_case where Id = CaseId)) AS handler,(select AssignedDate from Cus_case where Id = CaseId)AS AssignedDate";
            string table = "Cus_Workflow";

            var dtFinished = dbm.GetDataTable(field, table, htCondition, "Id");

            dtFinished.TableName = "Cus_Workflow";
            dtFinished.Columns["Id"].Caption = "编号";
            dtFinished.Columns["Created"].Caption = "建单人";
            dtFinished.Columns["Caseleibie"].Caption = "Case类别";
            dtFinished.Columns["CaseNo"].Caption = "Case编号";
            dtFinished.Columns["OneValue"].Caption = "一级Case类型";
            dtFinished.Columns["TwoValue"].Caption = "二级Case类型";
            dtFinished.Columns["ThreeValue"].Caption = "三级Case类型";
            dtFinished.Columns["FourValue"].Caption = "四级Case类型";
            dtFinished.Columns["Ajtype"].Caption = "案件类型";
            dtFinished.Columns["CustomerNo"].Caption = "客户编号";
            dtFinished.Columns["State"].Caption = "状态";
            dtFinished.Columns["Source"].Caption = "来源";
            dtFinished.Columns["Caselevel"].Caption = "故障级别";
            dtFinished.Columns["Priority"].Caption = "优先级";
            dtFinished.Columns["Stringent"].Caption = "紧急性";
            dtFinished.Columns["Enineer"].Caption = "工程师";
            dtFinished.Columns["handler"].Caption = "处理人";
            dtFinished.Columns["AssignedDate"].Caption = "建单时间";
            dtFinished.PrimaryKey = new DataColumn[] { dtFinished.Columns["Id"] };

            dtFinished.AcceptChanges();

            return dtFinished;
        }

        /// <summary>
        /// 客户导出Xml
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private DataTable getCustomerInfoTable(HttpContext context)
        {
            var dbm = DataBaseFactory.Instance.Create();

            var htCondition = new Hashtable();

            string CustomerNumber = context.Request["CustomerNumber"] ?? "";
            string CustomerName = context.Request["CustomerName"] ?? "";
            string Office = context.Request["Office"] ?? "";
            string telphone = context.Request["telphone"] ?? "";
            string Mobile = context.Request["Mobile"] ?? "";

            if (CustomerNumber != "") { htCondition.Add("CustomerNumber", CustomerNumber); }
            if (CustomerName != "") { htCondition.Add("CustomerName", CustomerName); }
            if (Office != "") { htCondition.Add("Office", Office); }
            if (telphone != "") { htCondition.Add("telphone", telphone); }
            if (Mobile != "") { htCondition.Add("Mobile", Mobile); }
            // ===
            string field = "*";
            string table = "Cus_Users";

            var dtCustomer = dbm.GetDataTable(field, table, htCondition, "Id");

            dtCustomer.TableName = "Cus_Users";
            dtCustomer.Columns["Id"].Caption = "编号";
            dtCustomer.Columns["CustomerName"].Caption = "客户姓名";
            dtCustomer.Columns["CustomerNumber"].Caption = "客户编号";
            dtCustomer.Columns["Post"].Caption = "邮编";
            dtCustomer.Columns["Email"].Caption = "邮箱";
            dtCustomer.Columns["telphone"].Caption = "联系电话";
            dtCustomer.Columns["Mobile"].Caption = "手机";
            dtCustomer.Columns["WorkZone"].Caption = "园区工贸";
            dtCustomer.Columns["Division"].Caption = "事业部";
            dtCustomer.Columns["Floor"].Caption = "楼层";
            dtCustomer.Columns["Vip"].Caption = "VIP";
            dtCustomer.Columns["Office"].Caption = "办公室";
            dtCustomer.Columns["Gallery"].Caption = "楼座";
            dtCustomer.Columns["AssetType"].Caption = "资产类型";
            dtCustomer.Columns["AssetNumber"].Caption = "资产编号";
            dtCustomer.PrimaryKey = new DataColumn[] { dtCustomer.Columns["Id"] };

            dtCustomer.AcceptChanges();

            return dtCustomer;
        }

        /// <summary>
        /// 客户历史工单导出报表
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private DataTable getCusHistory(HttpContext context)
        {
            

            var dbm = DataBaseFactory.Instance.Create();

            string CusNumber = context.Request["CusNumber"] ?? "";

            Hashtable htCondition = new Hashtable();

            if (CusNumber != "") { htCondition.Add("CustomerNo", CusNumber); }
            // ===
            string field = "Id,(select Text from Cus_CaseType where Id =OneValue) AS OneValue ,(select Text from Cus_CaseType where Id = TwoValue) AS TwoValue,(select Text from Cus_CaseType where Id = ThreeValue) AS ThreeValue,(select Text from Cus_CaseType where Id = FourValue) AS FourValue,(select Name from Sys_User where Id = Created) AS Created,Caseleibie,CaseNo,(select Casetype from Cus_AjType where Id= Ajtype ) AS Ajtype,CustomerNo,State,(select Sourcetext from Cus_source where Id = Source) AS Source,(select Caseleveltext from Cus_caseLevel where Id = Caselevel ) AS Caselevel,(select Prioritytext from Cus_priority where Id = Priority) AS Priority,(select Stringenttext from Cus_stringent where Id = Stringent ) AS Stringent,Enineer,(select Name from Sys_User where Id = handler ) AS handler, AssignedDate ";
            string table = "Cus_case";

            var dtCusHistory = dbm.GetDataTable(field, table, htCondition, "Id");

            dtCusHistory.TableName = "Cus_case";
            dtCusHistory.Columns["Id"].Caption = "编号";
            dtCusHistory.Columns["Created"].Caption = "建单人";
            dtCusHistory.Columns["Caseleibie"].Caption = "Case类别";
            dtCusHistory.Columns["CaseNo"].Caption = "Case编号";
            dtCusHistory.Columns["OneValue"].Caption = "一级Case类型";
            dtCusHistory.Columns["TwoValue"].Caption = "二级Case类型";
            dtCusHistory.Columns["ThreeValue"].Caption = "三级Case类型";
            dtCusHistory.Columns["FourValue"].Caption = "四级Case类型";
            dtCusHistory.Columns["Ajtype"].Caption = "案件类型";
            dtCusHistory.Columns["CustomerNo"].Caption = "客户编号";
            dtCusHistory.Columns["State"].Caption = "状态";
            dtCusHistory.Columns["Source"].Caption = "来源";
            dtCusHistory.Columns["Caselevel"].Caption = "故障级别";
            dtCusHistory.Columns["Priority"].Caption = "优先级";
            dtCusHistory.Columns["Stringent"].Caption = "紧急性";
            dtCusHistory.Columns["Enineer"].Caption = "工程师";
            dtCusHistory.Columns["handler"].Caption = "处理人";
            dtCusHistory.Columns["AssignedDate"].Caption = "建单时间";
            dtCusHistory.PrimaryKey = new DataColumn[] { dtCusHistory.Columns["Id"] };

            dtCusHistory.AcceptChanges();

            return dtCusHistory;
        }

        /// <summary>
        /// Case工单列表导出报表
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private DataTable getCaseListInfoTable(HttpContext context)
        {
            var dbm = DataBaseFactory.Instance.Create();

            string OneValue = context.Request["OneValue"] ?? "";
            string TwoValue = context.Request["TwoValue"] ?? "";
            string ThreeValue = context.Request["ThreeValue"] ?? "";
            string FourValue = context.Request["FourValue"] ?? "";
            string Ajtype = context.Request["Ajtype"] ?? "";
            string Caselevel = context.Request["Caselevel"] ?? "";
            string Priority = context.Request["Priority"] ?? "";
            string Stringent = context.Request["Stringent"] ?? "";
            string State = context.Request["State"] ?? "";
            string Created = context.Request["Created"] ?? "";
            string Enineer = context.Request["Enineer"] ?? "";
            string CustomerNo = context.Request["CustomerNo"] ?? "";
            string CaseNo = context.Request["CaseNo"] ?? "";

            Hashtable htCondition = new Hashtable();
            Hashtable htSysUser = new Hashtable();
            htCondition.Add("MainCaseNumber", "0");
            if (OneValue != "" && OneValue != "0") { htCondition.Add("OneValue", OneValue); }
            if (OneValue == "0") { }

            if (TwoValue != "" && TwoValue != "0") { htCondition.Add("TwoValue", TwoValue); }
            if (TwoValue == "0") { }

            if (ThreeValue != "" && ThreeValue != "0") { htCondition.Add("ThreeValue", ThreeValue); }
            if (ThreeValue == "0") { }

            if (FourValue != "" && FourValue == "0") { htCondition.Add("FourValue", FourValue); }
            if (FourValue == "0") { }

            if (Ajtype != "" && Ajtype != "0") { htCondition.Add("Ajtype", Ajtype); }
            if (Ajtype == "0") { }

            if (Caselevel != "" && Caselevel != "0") { htCondition.Add("Caselevel", Caselevel); }
            if (Caselevel == "0") { }

            if (Priority != "" && Priority != "0") { htCondition.Add("Priority", Priority); }
            if (Priority == "0") { }

            if (Stringent != "" && Stringent != "0") { htCondition.Add("Stringent", Stringent); }
            if (Stringent == "0") { }

            if (State != "" && State != "0") { htCondition.Add("State", State); }
            if (State == "0") { }

            //////////////输入的是名字，数据库里存的是ID所以要处理一下//////////////

            if (Created != "")
            {
                htSysUser.Add("Name", Created);
                DataTable dtSysUser = dbm.GetDataTable("*", "Sys_User", htSysUser, "Id");
                foreach (DataRow row in dtSysUser.Rows)
                {
                    string id = row["Id"].ToString();
                    htCondition.Add("Created", id);
                }

            }

            if (Enineer != "") { htCondition.Add("Enineer", Enineer); }
            if (CustomerNo != "") { htCondition.Add("CustomerNo", CustomerNo); }
            if (CaseNo != "") { htCondition.Add("CaseNo", CaseNo); }

            // ===
            string field = "Id,(select Text from Cus_CaseType where Id =OneValue) AS OneValue ,(select Text from Cus_CaseType where Id = TwoValue) AS TwoValue,(select Text from Cus_CaseType where Id = ThreeValue) AS ThreeValue,(select Text from Cus_CaseType where Id = FourValue) AS FourValue,(select Name from Sys_User where Id = Created) AS Created,Caseleibie,CaseNo,(select Casetype from Cus_AjType where Id= Ajtype ) AS Ajtype,CustomerNo,State,(select Sourcetext from Cus_source where Id = Source) AS Source,(select Caseleveltext from Cus_caseLevel where Id = Caselevel ) AS Caselevel,(select Prioritytext from Cus_priority where Id = Priority) AS Priority,(select Stringenttext from Cus_stringent where Id = Stringent ) AS Stringent,Enineer,(select Name from Sys_User where Id = handler ) AS handler, AssignedDate ";
            string table = "Cus_case";

            var dtCusCaseList = dbm.GetDataTable(field, table, htCondition, "Id");

            dtCusCaseList.TableName = "Cus_case";
            dtCusCaseList.Columns["Id"].Caption = "编号";
            dtCusCaseList.Columns["Created"].Caption = "建单人";
            dtCusCaseList.Columns["Caseleibie"].Caption = "Case类别";
            dtCusCaseList.Columns["CaseNo"].Caption = "Case编号";
            dtCusCaseList.Columns["OneValue"].Caption = "一级Case类型";
            dtCusCaseList.Columns["TwoValue"].Caption = "二级Case类型";
            dtCusCaseList.Columns["ThreeValue"].Caption = "三级Case类型";
            dtCusCaseList.Columns["FourValue"].Caption = "四级Case类型";
            dtCusCaseList.Columns["Ajtype"].Caption = "案件类型";
            dtCusCaseList.Columns["CustomerNo"].Caption = "客户编号";
            dtCusCaseList.Columns["State"].Caption = "状态";
            dtCusCaseList.Columns["Source"].Caption = "来源";
            dtCusCaseList.Columns["Caselevel"].Caption = "故障级别";
            dtCusCaseList.Columns["Priority"].Caption = "优先级";
            dtCusCaseList.Columns["Stringent"].Caption = "紧急性";
            dtCusCaseList.Columns["Enineer"].Caption = "工程师";
            dtCusCaseList.Columns["handler"].Caption = "处理人";
            dtCusCaseList.Columns["AssignedDate"].Caption = "建单时间";
            dtCusCaseList.PrimaryKey = new DataColumn[] { dtCusCaseList.Columns["Id"] };
            dtCusCaseList.AcceptChanges();

            return dtCusCaseList;
        }

        /// <summary>
        /// 工单预警导出报表
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private DataTable getcaseYujingInfoTable(HttpContext context)
        {


            var dbm = DataBaseFactory.Instance.Create();

            string CaseNo = context.Request["CaseNo"] ?? "";
            string Fazhi = context.Request["Fazhi"] ?? "";//前台输入的时间一小时为单位
            string cboState = context.Request["cboState"] ?? "";

            Hashtable htCondition = new Hashtable();
            htCondition.Add("State <> @StateProcessed", new SqlParameter("@StateProcessed", "Processed"));
            htCondition.Add("State <> @StateComplete", new SqlParameter("@StateComplete", "Complete"));
            if (CaseNo != "") { htCondition.Add("CaseNo", CaseNo); }
            if (cboState != "") { htCondition.Add("State", cboState); }

            // 首先你需要判断这个是否为空，不为空的话必须保证这个是数字
            int outFazhi = 0;
            if (Fazhi != "" && int.TryParse(Fazhi, out outFazhi))
            {
                DateTime startDate = DateTime.Now.AddHours(0 - outFazhi);
                htCondition.Add("AssignedDate <= @AssignedDate", new SqlParameter("@AssignedDate", startDate));
            }

            // ===
            string field = "Id,(select Text from Cus_CaseType where Id =OneValue) AS OneValue ,(select Text from Cus_CaseType where Id = TwoValue) AS TwoValue,(select Text from Cus_CaseType where Id = ThreeValue) AS ThreeValue,(select Text from Cus_CaseType where Id = FourValue) AS FourValue,(select Name from Sys_User where Id = Created) AS Created,Caseleibie,CaseNo,(select Casetype from Cus_AjType where Id= Ajtype ) AS Ajtype,CustomerNo,State,(select Sourcetext from Cus_source where Id = Source) AS Source,(select Caseleveltext from Cus_caseLevel where Id = Caselevel ) AS Caselevel,(select Prioritytext from Cus_priority where Id = Priority) AS Priority,(select Stringenttext from Cus_stringent where Id = Stringent ) AS Stringent,Enineer,(select Name from Sys_User where Id = handler ) AS handler, AssignedDate ";
            string table = "Cus_case";

            var dtCusCaseYujing = dbm.GetDataTable(field, table, htCondition, "Id");

            dtCusCaseYujing.TableName = "Cus_case";
            dtCusCaseYujing.Columns["Id"].Caption = "编号";
            dtCusCaseYujing.Columns["Created"].Caption = "建单人";
            dtCusCaseYujing.Columns["Caseleibie"].Caption = "Case类别";
            dtCusCaseYujing.Columns["CaseNo"].Caption = "Case编号";
            dtCusCaseYujing.Columns["OneValue"].Caption = "一级Case类型";
            dtCusCaseYujing.Columns["TwoValue"].Caption = "二级Case类型";
            dtCusCaseYujing.Columns["ThreeValue"].Caption = "三级Case类型";
            dtCusCaseYujing.Columns["FourValue"].Caption = "四级Case类型";
            dtCusCaseYujing.Columns["Ajtype"].Caption = "案件类型";
            dtCusCaseYujing.Columns["CustomerNo"].Caption = "客户编号";
            dtCusCaseYujing.Columns["State"].Caption = "状态";
            dtCusCaseYujing.Columns["Source"].Caption = "来源";
            dtCusCaseYujing.Columns["Caselevel"].Caption = "故障级别";
            dtCusCaseYujing.Columns["Priority"].Caption = "优先级";
            dtCusCaseYujing.Columns["Stringent"].Caption = "紧急性";
            dtCusCaseYujing.Columns["Enineer"].Caption = "工程师";
            dtCusCaseYujing.Columns["handler"].Caption = "处理人";
            dtCusCaseYujing.Columns["AssignedDate"].Caption = "建单时间";
            dtCusCaseYujing.PrimaryKey = new DataColumn[] { dtCusCaseYujing.Columns["Id"] };
            dtCusCaseYujing.AcceptChanges();

            return dtCusCaseYujing;
        }

        /// <summary>
        /// 快捷Case导出Xml报表
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private DataTable getQuickCase(HttpContext context)
        {

            var dbm = DataBaseFactory.Instance.Create();

            string CboOneLevel = context.Request["CboOneLevel"] ?? "";
            if (CboOneLevel == "请选择") { CboOneLevel = ""; }

            string CboTwoLevel = context.Request["CboTwoLevel"] ?? "";
            if (CboTwoLevel == "请选择") { CboOneLevel = ""; }

            string CboThreeLevel = context.Request["CboThreeLevel"] ?? "";
            if (CboThreeLevel == "请选择") { CboOneLevel = ""; }

            string CboFourLevel = context.Request["CboFourLevel"] ?? "";
            if (CboFourLevel == "请选择") { CboOneLevel = ""; }

            string CboAjLevel = context.Request["CboAjLevel"] ?? "";
            if (CboAjLevel == "请选择") { CboOneLevel = ""; }

            string CboGzLevel = context.Request["CboGzLevel"] ?? "";


            Hashtable htCondition = new Hashtable();

            if (CboOneLevel != "") { htCondition.Add("onevalue", CboOneLevel); }
            if (CboTwoLevel != "") { htCondition.Add("twovalue", CboTwoLevel); }
            if (CboThreeLevel != "") { htCondition.Add("threevalue", CboThreeLevel); }
            if (CboFourLevel != "") { htCondition.Add("fourvalue", CboFourLevel); }
            if (CboAjLevel != "") { htCondition.Add("casetypevalue", CboAjLevel); }
            if (CboGzLevel != "") { htCondition.Add("casedescription like @casedescription", new SqlParameter("@casedescription", "%" + CboGzLevel + "%")); }

            // ===
            string field = "Id,(select Text from Cus_CaseType where Id =OneValue) AS OneValue ,(select Text from Cus_CaseType where Id = TwoValue) AS TwoValue,(select Text from Cus_CaseType where Id = ThreeValue) AS ThreeValue,(select Text from Cus_CaseType where Id = FourValue) AS FourValue, Created,(select Casetype from Cus_AjType where Id= casetypevalue ) AS Ajtype,(select Sourcetext from Cus_source where Id = sourcevalue) AS Source,(select Caseleveltext from Cus_caseLevel where Id = caselevelvalue ) AS Caselevel,(select Prioritytext from Cus_priority where Id = priorityvalue) AS Priority,(select Stringenttext from Cus_stringent where Id = stringentvalue ) AS Stringent, solutions,worklog,createDate ";
            string table = "Cus_QuickCase";

            var dtQuickCase = dbm.GetDataTable(field, table, htCondition, "Id");

            dtQuickCase.TableName = "Cus_case";
            dtQuickCase.Columns["Id"].Caption = "编号";
            dtQuickCase.Columns["Created"].Caption = "建单人";
            dtQuickCase.Columns["OneValue"].Caption = "一级Case类型";
            dtQuickCase.Columns["TwoValue"].Caption = "二级Case类型";
            dtQuickCase.Columns["ThreeValue"].Caption = "三级Case类型";
            dtQuickCase.Columns["FourValue"].Caption = "四级Case类型";
            dtQuickCase.Columns["Ajtype"].Caption = "案件类型";
            dtQuickCase.Columns["Source"].Caption = "来源";
            dtQuickCase.Columns["Caselevel"].Caption = "故障级别";
            dtQuickCase.Columns["Priority"].Caption = "优先级";
            dtQuickCase.Columns["Stringent"].Caption = "紧急性";
            dtQuickCase.Columns["createDate"].Caption = "建单时间";
            dtQuickCase.PrimaryKey = new DataColumn[] { dtQuickCase.Columns["Id"] };
            dtQuickCase.AcceptChanges();

            return dtQuickCase;
        }
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}