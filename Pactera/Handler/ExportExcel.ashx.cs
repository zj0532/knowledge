using NPOI.HSSF.UserModel;
using Pactera.Common.Npoi;
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
    public class ExportExcel : Pactera.Web.BaseHttpHandler, IHttpHandler

    {
        public void ProcessRequest(HttpContext context)
        {
            Excel exc = new Excel();
            DataTable dtExport = new DataTable();

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
                    dtExport = getfinishedInfoTable(context);
                    break;
                case "Customer":
                    dtExport = getCustomerInfoTable(context);
                    break;
                case "CusHistory":
                    dtExport = getCusHistoryInfoTable(context);
                    break;
                case "CaseList":
                    dtExport = getCaseListInfoTable(context);
                    break;
                case "CaseYujing":
                    dtExport = getCaseYujingInfoTable(context);
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
                byte[] data = exc.TableToExcel(dtExport, sheetname);

                // 设置头值
                context.Response.ContentType = "application/nvd.ms-excel";
                context.Response.Headers.Add("content-disposition:", "attachment; filename=" + filename); 

                // 输出文件到客户端
                context.Response.BinaryWrite(data);
                context.Response.End();
            }
            else
            {
                byte[] data = exc.TableToExcel(new DataTable(), sheetname);
                // 设置头值
                context.Response.ContentType = "application/nvd.ms-excel";
                context.Response.Headers.Add("content-disposition:", "attachment; filename=" + filename);

                // 输出文件到客户端
                context.Response.BinaryWrite(data);
                context.Response.End();
            }
        }
        

        /// <summary>
        /// 导出待办任务列表内容
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
            string field = "Id As '编号',(select Text from Cus_CaseType where Id =(select OneValue from Cus_case where Id = CaseId))AS '一级Case类型',(select Text from Cus_CaseType where Id =(select TwoValue from Cus_case where Id = CaseId))AS '二级Case类型',(select Text from Cus_CaseType where Id =(select ThreeValue from Cus_case where Id = CaseId))AS '三级Case类型',(select Text from Cus_CaseType where Id =(select FourValue from Cus_case where Id = CaseId))AS '四级Case类型',(select Name from Sys_User where Id = (select Created from Cus_case where Id = CaseId)) AS '建单人',(select Caseleibie from Cus_case where Id=CaseId )AS 'Case类别',(select CaseNo from Cus_case where Id=CaseId) AS 'Case编号',(select Casetype from Cus_AjType where Id=(select Ajtype from Cus_case where Id=CaseId)) AS '案件类型',(select CustomerNo from Cus_case where Id=CaseId) AS '客户编号',(select State from Cus_case where Id=CaseId) AS '工单状态',(select Sourcetext from Cus_source where Id = (select Source from Cus_case where Id=CaseId)) AS '来源',(select Caseleveltext from Cus_caseLevel where Id = (select Caselevel from Cus_case where Id=CaseId)) AS '故障级别',(select Prioritytext from Cus_priority where Id = (select Priority from Cus_case where Id=CaseId)) AS '优先级',(select Stringenttext from Cus_stringent where Id = (select Stringent from Cus_case where Id=CaseId)) AS '紧急性',(select Enineer from Cus_case where Id = CaseId) AS '工程师',(select Name from Sys_User where Id = (select handler from Cus_case where Id = CaseId)) AS '处理人',(select AssignedDate from Cus_case where Id = CaseId)AS '建单时间'";
            string table = "Cus_Workflow";

            var dtCase = dbm.GetDataTable(field, table, htCondition, "Id");

            return dtCase;
        }

        /// <summary>
        /// 导出报表已办未完成任务
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
            string field = "Id As '编号',(select Text from Cus_CaseType where Id =(select OneValue from Cus_case where Id = CaseId))AS '一级Case类型',(select Text from Cus_CaseType where Id =(select TwoValue from Cus_case where Id = CaseId))AS '二级Case类型',(select Text from Cus_CaseType where Id =(select ThreeValue from Cus_case where Id = CaseId))AS '三级Case类型',(select Text from Cus_CaseType where Id =(select FourValue from Cus_case where Id = CaseId))AS '四级Case类型',(select Name from Sys_User where Id = (select Created from Cus_case where Id = CaseId)) AS '建单人',(select Caseleibie from Cus_case where Id=CaseId )AS 'Case类别',(select CaseNo from Cus_case where Id=CaseId) AS 'Case编号',(select Casetype from Cus_AjType where Id=(select Ajtype from Cus_case where Id=CaseId)) AS '案件类型',(select CustomerNo from Cus_case where Id=CaseId) AS '客户编号',(select State from Cus_case where Id=CaseId) AS '工单状态',(select Sourcetext from Cus_source where Id = (select Source from Cus_case where Id=CaseId)) AS '来源',(select Caseleveltext from Cus_caseLevel where Id = (select Caselevel from Cus_case where Id=CaseId)) AS '故障级别',(select Prioritytext from Cus_priority where Id = (select Priority from Cus_case where Id=CaseId)) AS '优先级',(select Stringenttext from Cus_stringent where Id = (select Stringent from Cus_case where Id=CaseId)) AS '紧急性',(select Enineer from Cus_case where Id = CaseId) AS '工程师',(select Name from Sys_User where Id = (select handler from Cus_case where Id = CaseId)) AS '处理人',(select AssignedDate from Cus_case where Id = CaseId)AS '建单时间'";
            string table = "Cus_Workflow";

            var UnFinished = dbm.GetDataTable(field, table, htCondition, "Id");

            return UnFinished;
        }


        /// <summary>
        /// 导出报表已办未完成任务
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private DataTable getfinishedInfoTable(HttpContext context)
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
            string field = "Id As '编号',(select Text from Cus_CaseType where Id =(select OneValue from Cus_case where Id = CaseId))AS '一级Case类型',(select Text from Cus_CaseType where Id =(select TwoValue from Cus_case where Id = CaseId))AS '二级Case类型',(select Text from Cus_CaseType where Id =(select ThreeValue from Cus_case where Id = CaseId))AS '三级Case类型',(select Text from Cus_CaseType where Id =(select FourValue from Cus_case where Id = CaseId))AS '四级Case类型',(select Name from Sys_User where Id = (select Created from Cus_case where Id = CaseId)) AS '建单人',(select Caseleibie from Cus_case where Id=CaseId )AS 'Case类别',(select CaseNo from Cus_case where Id=CaseId) AS 'Case编号',(select Casetype from Cus_AjType where Id=(select Ajtype from Cus_case where Id=CaseId)) AS '案件类型',(select CustomerNo from Cus_case where Id=CaseId) AS '客户编号',(select State from Cus_case where Id=CaseId) AS '工单状态',(select Sourcetext from Cus_source where Id = (select Source from Cus_case where Id=CaseId)) AS '来源',(select Caseleveltext from Cus_caseLevel where Id = (select Caselevel from Cus_case where Id=CaseId)) AS '故障级别',(select Prioritytext from Cus_priority where Id = (select Priority from Cus_case where Id=CaseId)) AS '优先级',(select Stringenttext from Cus_stringent where Id = (select Stringent from Cus_case where Id=CaseId)) AS '紧急性',(select Enineer from Cus_case where Id = CaseId) AS '工程师',(select Name from Sys_User where Id = (select handler from Cus_case where Id = CaseId)) AS '处理人',(select AssignedDate from Cus_case where Id = CaseId)AS '建单时间'";
            string table = "Cus_Workflow";

            var Finished = dbm.GetDataTable(field, table, htCondition, "Id");

            return Finished;
        }

        /// <summary>
        /// 导出客户报表
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private DataTable getCustomerInfoTable(HttpContext context)
        {


            var dbm = DataBaseFactory.Instance.Create();

            string CustomerNumber = context.Request["CustomerNumber"] ?? "";
            string CustomerName = context.Request["CustomerName"] ?? "";
            string Office = context.Request["Office"] ?? "";
            string telphone = context.Request["telphone"] ?? "";
            string Mobile = context.Request["Mobile"] ?? "";

            Hashtable htCondition = new Hashtable();

            if (CustomerNumber != "") { htCondition.Add("CustomerNumber", CustomerNumber); }
            if (CustomerName != "") { htCondition.Add("CustomerName", CustomerName); }
            if (Office != "") { htCondition.Add("Office", Office); }
            if (telphone != "") { htCondition.Add("telphone", telphone); }
            if (Mobile != "") { htCondition.Add("Mobile", Mobile); }

            // ===
            string field = " Id As '编号',CustomerNumber AS'客户编号',CustomerName AS '客户名称',Post AS '邮编',Email AS '邮箱',telphone AS '联系电话',Mobile AS '手机',WorkZone AS '园区工贸',Division AS '事业部',Floor AS '楼层',Vip AS 'VIP',Office AS '办公地点',Gallery AS '楼座',AssetType AS '资产类型',AssetNumber AS '资产编号'";
            string table = "Cus_Users";

            var dtCustomer = dbm.GetDataTable(field, table, htCondition, "Id");

            return dtCustomer;
        }
        /// <summary>
        /// 客户历史记录导出报表
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private DataTable getCusHistoryInfoTable(HttpContext context)
        {
            

            var dbm = DataBaseFactory.Instance.Create();

            string CustomerNumber = context.Request["CustomerNumber"] ?? "";
            string CustomerName = context.Request["CustomerName"] ?? "";
            string Office = context.Request["Office"] ?? "";
            string telphone = context.Request["telphone"] ?? "";
            string Mobile = context.Request["Mobile"] ?? "";

            Hashtable htCondition = new Hashtable();

            if (CustomerNumber != "") { htCondition.Add("CustomerNumber", CustomerNumber); }
            if (CustomerName != "") { htCondition.Add("CustomerName", CustomerName); }
            if (Office != "") { htCondition.Add("Office", Office); }
            if (telphone != "") { htCondition.Add("telphone", telphone); }
            if (Mobile != "") { htCondition.Add("Mobile", Mobile); }

            // ===
            string field = "Id As '编号',(select Text from Cus_CaseType where Id =OneValue) AS '一级Case类型',(select Text from Cus_CaseType where Id = TwoValue) AS '二级Case类型',(select Text from Cus_CaseType where Id = ThreeValue) AS '三级Case类型',(select Text from Cus_CaseType where Id = FourValue) AS '四级Case类型',(select Name from Sys_User where Id = Created) AS '建单人',Caseleibie AS 'Case类别',CaseNo AS 'Case编号',(select Casetype from Cus_AjType where Id= Ajtype ) AS '案件类型',CustomerNo AS '客户编号',State AS '工单状态',(select Sourcetext from Cus_source where Id = Source) AS '来源',(select Caseleveltext from Cus_caseLevel where Id = Caselevel ) AS '故障级别',(select Prioritytext from Cus_priority where Id = Priority) AS '优先级',(select Stringenttext from Cus_stringent where Id = Stringent ) AS '紧急性',Enineer AS '工程师',(select Name from Sys_User where Id = handler ) AS '处理人', AssignedDate AS '建单时间'";
            string table = "Cus_case";

            var dtCustomer = dbm.GetDataTable(field, table, htCondition, "Id");

            return dtCustomer;
        }
        /// <summary>
        /// 工单列表导出Excel报表
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
            string field = "Id As '编号',(select Text from Cus_CaseType where Id =OneValue) AS '一级Case类型',(select Text from Cus_CaseType where Id = TwoValue) AS '二级Case类型',(select Text from Cus_CaseType where Id = ThreeValue) AS '三级Case类型',(select Text from Cus_CaseType where Id = FourValue) AS '四级Case类型',(select Name from Sys_User where Id = Created) AS '建单人',Caseleibie AS 'Case类别',CaseNo AS 'Case编号',(select Casetype from Cus_AjType where Id= Ajtype ) AS '案件类型',CustomerNo AS '客户编号',State AS '工单状态',(select Sourcetext from Cus_source where Id = Source) AS '来源',(select Caseleveltext from Cus_caseLevel where Id = Caselevel ) AS '故障级别',(select Prioritytext from Cus_priority where Id = Priority) AS '优先级',(select Stringenttext from Cus_stringent where Id = Stringent ) AS '紧急性',Enineer AS '工程师',(select Name from Sys_User where Id = handler ) AS '处理人', AssignedDate AS '建单时间'";
            string table = "Cus_case";

            var dtCustomer = dbm.GetDataTable(field, table, htCondition, "Id");

            return dtCustomer;
        }

        /// <summary>
        /// 工单预警导出报表
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private DataTable getCaseYujingInfoTable(HttpContext context)
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
            string field = "Id As '编号',(select Text from Cus_CaseType where Id =OneValue) AS '一级Case类型',(select Text from Cus_CaseType where Id = TwoValue) AS '二级Case类型',(select Text from Cus_CaseType where Id = ThreeValue) AS '三级Case类型',(select Text from Cus_CaseType where Id = FourValue) AS '四级Case类型',(select Name from Sys_User where Id = Created) AS '建单人',Caseleibie AS 'Case类别',CaseNo AS 'Case编号',(select Casetype from Cus_AjType where Id= Ajtype ) AS '案件类型',CustomerNo AS '客户编号',State AS '工单状态',(select Sourcetext from Cus_source where Id = Source) AS '来源',(select Caseleveltext from Cus_caseLevel where Id = Caselevel ) AS '故障级别',(select Prioritytext from Cus_priority where Id = Priority) AS '优先级',(select Stringenttext from Cus_stringent where Id = Stringent ) AS '紧急性',Enineer AS '工程师',(select Name from Sys_User where Id = handler ) AS '处理人', AssignedDate AS '建单时间'";
            string table = "Cus_case";

            var dtCusCaseYujing = dbm.GetDataTable(field, table, htCondition, "Id");

            return dtCusCaseYujing;
        }

        /// <summary>
        /// 快捷Case导出Excel报表
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

            string files = "id AS '编号',(select Text from Cus_CaseType where Id = onevalue) as 'Case类型一级',(select Text from Cus_CaseType where Id = twovalue) as 'Case类型二级',(select Text from Cus_CaseType where Id = threevalue) as 'Case类型三级',(select Text from Cus_CaseType where Id = fourvalue) as 'Case类型四级',(select Sourcetext from Cus_source where Id = sourcevalue) as '来源',(select Caseleveltext from Cus_caseLevel where Id = caselevelvalue) as '故障级别',(select Prioritytext from Cus_priority where Id = priorityvalue) as '优先级',(select Stringenttext from Cus_stringent where Id = stringentvalue) as '紧急性',createDate as '建单时间',casedescription as '故障描述',solutions as '解决方案',worklog as '工作日志',Created AS '建单人'";

            string table = "Cus_QuickCase";

            var dtCusCaseYujing = dbm.GetDataTable(files, table, htCondition, "Id");

            return dtCusCaseYujing;
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