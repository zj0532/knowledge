using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using System.IO;
using System.Web.Services;
using Pactera.Data;
using System.Text;
using System.Data;

namespace Pactera.Handler
{
    /// <summary>
    /// 文件上传处理程序
    /// </summary>
    public class PublicFileUpload : Pactera.Web.BaseHttpHandler, IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            // TODO 公共 - 文件上传处理程序 - 这里是否需要考虑权限验证

            context.Response.ContentType = "text/plain";
            context.Response.Charset = "utf-8";

            if(context.Request["action"] == "delete_uploaded_file")
            {
                int id = 0;
                int.TryParse(context.Request["id"], out id);

                if (id == 0) return;

                var dbm = DataBaseFactory.Instance.Create();

                var dtAttachment = dbm.GetDataTable("Name,Extension", "BPM_Attachment", id.ToString(), "Id");
                if (dtAttachment.Rows.Count < 1) return;

                string path = dbm.GetFieldValue("BPM_Attachment", "RelativePath", id.ToString(), "Id");
                path = context.Server.MapPath(path);

                // 删除数据
                dbm.Delete("BPM_Attachment", id.ToString(), "Id");

                if(File.Exists(path))
                {
                    try
                    {
                        File.Delete(path);
                    }
                    catch (Exception)
                    {

                    }
                }

                return;
                //string fiele = dbm.GetFieldValue("BPM_Attachment","")
            }

            HttpPostedFile file = context.Request.Files["Filedata"];
            string uploadPath = context.Server.MapPath("~/Upload/Attachment/");

            if (file != null)
            {
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                // 1 处理文件格式
                string name = DateTime.Now.ToString("yyyyMMddHHmmssffff");
                string ext = Path.GetExtension(file.FileName);
                file.SaveAs(uploadPath + name + ext);

                // 2 文件保存到数据库表【BPM_Attachment（附件表）】中
                Hashtable htField = new Hashtable();
                htField.Add("Name", name);
                htField.Add("Extension", ext);
                htField.Add("Size", file.InputStream.Length);
                htField.Add("OriFileName", file.FileName);
                htField.Add("RelativePath", "/Upload/Attachment/" + name + ext);
                // TODO 张春雨 - 文件上传处理程序 - 这里取真实用户时报错，可能是提交时是异步提交获取不到Session
                //htField.Add("CreateUserId", CurrentSigninUser.Id);
                htField.Add("CreateUserId", "0");

                int id = 0;
                var dbm = DataBaseFactory.Instance.Create();
                dbm.Insert("BPM_Attachment", htField, out id);
                //var returnstr = id + ";/Upload/Attachment/" + name + ext;

                StringBuilder sbHtml = new StringBuilder();
                sbHtml.AppendFormat("<div id=\"uploaded-queue-item_{0}\" class=\"uploaded-queue-item\">", id);
                sbHtml.AppendFormat("<input type=\"hidden\" name=\"hidFileId\" value=\"{0}\" />", id);
                sbHtml.AppendFormat("<span>{0}</span>", file.FileName);
                sbHtml.AppendFormat("<a href=\"javascript:void(0)\" onclick=\"deleteUploadedFile('{0}')\"></a>", id);
                sbHtml.Append("</div>");

                // 3 返回刚才保存的记录ID
                context.Response.Write(sbHtml.ToString());
            }
            else
                // 如果为空，处理相应的结果
                context.Response.Write(0);
        }

        private void MakeThumbnail(string sourcePath, string newPath, int width, int height)
        {
            System.Drawing.Image ig = System.Drawing.Image.FromFile(sourcePath);
            int towidth = width;
            int toheight = height;
            int x = 0;
            int y = 0;
            int ow = ig.Width;
            int oh = ig.Height;
            if ((double)ig.Width / (double)ig.Height > (double)towidth / (double)toheight)
            {
                oh = ig.Height;
                ow = ig.Height * towidth / toheight;
                y = 0;
                x = (ig.Width - ow) / 2;

            }
            else
            {
                ow = ig.Width;
                oh = ig.Width * height / towidth;
                x = 0;
                y = (ig.Height - oh) / 2;
            }
            System.Drawing.Image bitmap = new System.Drawing.Bitmap(towidth, toheight);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.Clear(System.Drawing.Color.Transparent);
            g.DrawImage(ig, new System.Drawing.Rectangle(0, 0, towidth, toheight), new System.Drawing.Rectangle(x, y, ow, oh), System.Drawing.GraphicsUnit.Pixel);
            try
            {
                bitmap.Save(newPath, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                ig.Dispose();
                bitmap.Dispose();
                g.Dispose();
            }
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