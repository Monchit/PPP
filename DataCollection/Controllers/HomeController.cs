using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataCollection.Models;
using MvcJqGrid;
using System.Linq.Dynamic;
using System.IO;
using System.Web.UI;
using System.Globalization;

namespace DataCollection.Controllers
{
    public class HomeController : Controller
    {
        partnicsEntities db = new partnicsEntities();
        qimnicsEntities qim = new qimnicsEntities();
        Util util = new Util();

        public ActionResult Index()
        {
            //ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";
            return View();
        }

        private string convDateText(string dateStr, string timeStr)
        {
            if (!string.IsNullOrEmpty(dateStr.Trim()) && !string.IsNullOrEmpty(timeStr.Trim()))
                return dateStr.Trim().Substring(6, 2) + "/" + dateStr.Trim().Substring(4, 2) + "/" + dateStr.Trim().Substring(0, 4) + " " + timeStr.Substring(0, 2) + ":" + timeStr.Substring(2, 2) + ":" + timeStr.Substring(4, 2);
            else
                return string.Empty;
        }

        private string formatName(string originalName)
        {
            if (originalName != null)
            {
                var arrName = originalName.Trim().Split(' ');
                if (arrName.Length == 2)
                    return arrName[0] + " " + arrName[1].Substring(0, 2) + ".";
                else
                    return arrName[0];
            }
            else
            {
                return string.Empty;
            }
        }

        public JsonResult GetData(GridSettings gridSettings, string jobno, string lotno,
            string datefrom, string dateto, int minus, string wcCure, string wcPart,
            string itemPart, string itemFG, int status)
        {
            var data = (from c in qim.tr_curing_nics
                        join m in qim.tr_metal_job_nics
                        on c.job_order_no equals m.job_order_no into j
                        from m in j.DefaultIfEmpty()
                        where c.record_status == "A"
                        select new
                        {
                            job_order_no = c.job_order_no.Trim(),
                            lotno = c.tnc_total_tag_no.Trim() + "-" + c.tnc_tag_no.Trim(),
                            c.curing_date,
                            line_no_cure = c.line_no.Trim(),
                            line_no_part = m.line_no.Trim(),
                            parts = m.parts.Trim(),
                            finished_goods_code = m.finished_goods_code.Trim(),
                            m.issue_qty,
                            m.parts_job_order_no
                        }).OrderBy(gridSettings.SortColumn + " " + gridSettings.SortOrder);
            // Filter Condition
            if (!string.IsNullOrEmpty(jobno))
            {
                var partJobNo = jobno.Trim().Split('-')[0];
                data = data.Where(w => w.job_order_no != null && w.job_order_no.Contains(partJobNo));
            }

            if (!string.IsNullOrEmpty(lotno))
            {
                data = data.Where(w => w.lotno.Contains(lotno.Trim()));
            }

            if (!string.IsNullOrEmpty(datefrom))
            {
                var dfrom = util.convDateStrToInt(datefrom);
                data = data.Where(w => w.curing_date >= dfrom);
            }

            if (!string.IsNullOrEmpty(dateto))
            {
                var dto = util.convDateStrToInt(dateto);
                data = data.Where(w => w.curing_date <= dto);
            }

            if (!string.IsNullOrEmpty(wcCure))
            {
                data = data.Where(w => w.line_no_cure.Contains(wcCure.Trim()));
            }

            if (!string.IsNullOrEmpty(wcPart))
            {
                data = data.Where(w => w.line_no_part.Contains(wcPart.Trim()));
            }

            if (!string.IsNullOrEmpty(itemPart))
            {
                data = data.Where(w => w.parts.Contains(itemPart.Trim()));
            }

            if (!string.IsNullOrEmpty(itemFG))
            {
                data = data.Where(w => w.finished_goods_code.Contains(itemFG.Trim()));
            }
            
            // Select Data Per Page
            var today = util.convDateStrToInt(DateTime.Today.Day.ToString("00") + "/" + DateTime.Today.Month.ToString("00") + "/" + DateTime.Today.Year.ToString());
            //var loadingStatus = (from d in db.td_part_delivery.Where(w => w.loading_status.Trim() == "1").Select(s => new { part_job_order_no = s.part_job_order_no.Trim(), s.qty })
            //                     group d by new { d.part_job_order_no, d.qty } into g
            //                     select new
            //                     {
            //                         part_job_order_no = g.Key.part_job_order_no,
            //                         qty = g.Key.qty
            //                     }).ToList();

            var dataTmp = from d in data.ToList()
                          //join s in loadingStatus 
                          //on d.parts_job_order_no equals s.part_job_order_no into j
                          //from s in j.DefaultIfEmpty()
                          select new
                          {
                              d.job_order_no,
                              d.lotno,
                              d.line_no_cure,
                              d.parts,
                              d.finished_goods_code,
                              d.issue_qty,
                              curing_date = d.curing_date != null ? util.convDateIntToStr(d.curing_date) : "",
                              //loading = s != null ? s.part_job_order_no : "",
                              //qty = s != null ? s.qty : 0,
                              //status = s != null ?
                              //("<a href='#' class='lnkJob_Complete' data-jobNo='" + (d.parts_job_order_no != null ? d.parts_job_order_no.Trim() : d.parts_job_order_no) + "'><b>Details</b></a>") :
                              //(d.curing_date != null && d.curing_date <= (today + minus)) ?
                              //("<a href='#' class='lnkJob_PassDue' data-jobNo='" + (d.parts_job_order_no != null ? d.parts_job_order_no.Trim() : d.parts_job_order_no) + "'><b>Details</b></a>") :
                              //("<a href='#' class='lnkJob_Normal' data-jobNo='" + (d.parts_job_order_no != null ? d.parts_job_order_no.Trim() : d.parts_job_order_no) + "'><b>Details</b></a>")
                              //status = "<a href='#' class='lnkJob_Normal' data-jobNo='" + (d.parts_job_order_no ?? "") + "'><b>Details</b></a>"
                              status = (d.curing_date != null && d.curing_date <= (today + minus)) ?
                              ("<a href='#' class='lnkJob_PassDue' data-jobNo='" + (d.parts_job_order_no != null ? d.parts_job_order_no.Trim() : d.parts_job_order_no) + "'><b>Details</b></a>") :
                              ("<a href='#' class='lnkJob_Normal' data-jobNo='" + (d.parts_job_order_no != null ? d.parts_job_order_no.Trim() : d.parts_job_order_no) + "'><b>Normal</b></a>")
                          };

            if (status == 1)
            {
                dataTmp = dataTmp.Where(w => w.status.Contains("lnkJob_Normal"));
            }
            else if (status == 2)
            {
                dataTmp = dataTmp.Where(w => w.status.Contains("lnkJob_PassDue"));
            }
            else if (status == 3)
            {
                dataTmp = dataTmp.Where(w => w.status.Contains("lnkJob_Complete"));
            }

            // Count All Record
            double count = dataTmp.Count();

            var selectData = dataTmp.Skip((gridSettings.PageIndex - 1) * gridSettings.PageSize).Take(gridSettings.PageSize)
                .Select(d => new
                {
                    id = d.job_order_no,
                    cell = new object[]
                        {
                            d.line_no_cure,
                            d.finished_goods_code,
                            d.job_order_no,
                            d.lotno,
                            d.issue_qty,
                            d.parts,
                            d.curing_date,
                            d.status
                        }
                });

            //// Sum Record
            //var allQTYSum = dataTmp.Where(w => w.issue_qty != null).Sum(s => s.issue_qty);
            //var compQTYSum = double.Parse(dataTmp.Where(w => w.loading != null).Sum(s => s.qty).ToString());
            //// Completed Percent
            //var completedPercent = allQTYSum > 0 ? string.Format("{0:0.00}", (compQTYSum / allQTYSum) * 100) : "0.00";
            //var allQTYSumStr = string.Format("{0:n0}", allQTYSum);
            //var compQTYSumStr = string.Format("{0:n0}", compQTYSum);

            // Prepare JSON Format
            object jsonData = new
            {
                page = gridSettings.PageIndex,
                total = Math.Ceiling(count / gridSettings.PageSize),
                records = count,
                //all = allQTYSumStr,
                //comp = compQTYSumStr,
                //completedPercent = completedPercent,
                rows = selectData
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetJobDetails(string id)
        {
            var workCenter = qim.tm_workcenter;
            var routing = qim.tr_job_progress_nics.Where(w => w.parent_job_order_no != "" && w.parent_job_order_no.Trim() == id).FirstOrDefault();
            if (routing == null)
            {
                routing = qim.tr_job_progress_nics.Where(w => w.job_order_no.Trim() == id).FirstOrDefault();
            }
            var partJobNo = routing.job_order_no;
            var job_progress = db.td_part_progress.Where(w => w.job_order_no.Trim().Contains(partJobNo)).FirstOrDefault();
            var next_progress = db.tt_part_stat.Where(w => w.job_order_no.Trim().Contains(partJobNo)).FirstOrDefault();
            var delivery = db.td_part_delivery.Where(w => w.part_job_order_no.Trim().Contains(partJobNo)).FirstOrDefault();

            object jsondata;

            try
            {
                var routingObj = new
                    {
                        oc_00 = routing.operation_code_00,
                        oc_00_detail = workCenter.Where(w => w.wc.Trim() == routing.operation_code_00.Trim()).Select(s => s.wc_name).FirstOrDefault(),
                        img_oc_00 = isExistsPath(routing.operation_code_00),
                        oc_01 = routing.operation_code_01,
                        oc_01_detail = workCenter.Where(w => w.wc.Trim() == routing.operation_code_01.Trim()).Select(s => s.wc_name).FirstOrDefault(),
                        img_oc_01 = isExistsPath(routing.operation_code_01),
                        oc_02 = routing.operation_code_02,
                        oc_02_detail = workCenter.Where(w => w.wc.Trim() == routing.operation_code_02.Trim()).Select(s => s.wc_name).FirstOrDefault(),
                        img_oc_02 = isExistsPath(routing.operation_code_02),
                        oc_03 = routing.operation_code_03,
                        oc_03_detail = workCenter.Where(w => w.wc.Trim() == routing.operation_code_03.Trim()).Select(s => s.wc_name).FirstOrDefault(),
                        img_oc_03 = isExistsPath(routing.operation_code_03),
                        oc_04 = routing.operation_code_04,
                        oc_04_detail = workCenter.Where(w => w.wc.Trim() == routing.operation_code_04.Trim()).Select(s => s.wc_name).FirstOrDefault(),
                        img_oc_04 = isExistsPath(routing.operation_code_04),
                        oc_05 = routing.operation_code_05,
                        oc_05_detail = workCenter.Where(w => w.wc.Trim() == routing.operation_code_05.Trim()).Select(s => s.wc_name).FirstOrDefault(),
                        img_oc_05 = isExistsPath(routing.operation_code_05),
                        oc_06 = routing.operation_code_06,
                        oc_06_detail = workCenter.Where(w => w.wc.Trim() == routing.operation_code_06.Trim()).Select(s => s.wc_name).FirstOrDefault(),
                        img_oc_06 = isExistsPath(routing.operation_code_06),
                        oc_07 = routing.operation_code_07,
                        oc_07_detail = workCenter.Where(w => w.wc.Trim() == routing.operation_code_07.Trim()).Select(s => s.wc_name).FirstOrDefault(),
                        img_oc_07 = isExistsPath(routing.operation_code_07),
                        oc_08 = routing.operation_code_08,
                        oc_08_detail = workCenter.Where(w => w.wc.Trim() == routing.operation_code_08.Trim()).Select(s => s.wc_name).FirstOrDefault(),
                        oc_08oc_08 = isExistsPath(routing.operation_code_08),
                        oc_09 = routing.operation_code_09,
                        oc_09_detail = workCenter.Where(w => w.wc.Trim() == routing.operation_code_09.Trim()).Select(s => s.wc_name).FirstOrDefault(),
                        img_oc_09 = isExistsPath(routing.operation_code_09),
                        oc_10 = routing.operation_code_10,
                        oc_10_detail = workCenter.Where(w => w.wc.Trim() == routing.operation_code_10.Trim()).Select(s => s.wc_name).FirstOrDefault(),
                        img_oc_10 = isExistsPath(routing.operation_code_10),
                        oc_11 = routing.operation_code_11,
                        oc_11_detail = workCenter.Where(w => w.wc.Trim() == routing.operation_code_11.Trim()).Select(s => s.wc_name).FirstOrDefault(),
                        img_oc_11 = isExistsPath(routing.operation_code_11),
                        oc_12 = routing.operation_code_12,
                        oc_12_detail = workCenter.Where(w => w.wc.Trim() == routing.operation_code_12.Trim()).Select(s => s.wc_name).FirstOrDefault(),
                        img_oc_12 = isExistsPath(routing.operation_code_12),
                        oc_13 = routing.operation_code_13,
                        oc_13_detail = workCenter.Where(w => w.wc.Trim() == routing.operation_code_13.Trim()).Select(s => s.wc_name).FirstOrDefault(),
                        img_oc_13 = isExistsPath(routing.operation_code_13),
                        oc_14 = routing.operation_code_14,
                        oc_14_detail = workCenter.Where(w => w.wc.Trim() == routing.operation_code_14.Trim()).Select(s => s.wc_name).FirstOrDefault(),
                        img_oc_14 = isExistsPath(routing.operation_code_14),
                        oc_15 = routing.operation_code_15,
                        oc_15_detail = workCenter.Where(w => w.wc.Trim() == routing.operation_code_15.Trim()).Select(s => s.wc_name).FirstOrDefault(),
                        img_oc_15 = isExistsPath(routing.operation_code_15),
                        oc_16 = routing.operation_code_16,
                        oc_16_detail = workCenter.Where(w => w.wc.Trim() == routing.operation_code_16.Trim()).Select(s => s.wc_name).FirstOrDefault(),
                        img_oc_16 = isExistsPath(routing.operation_code_16),
                        oc_17 = routing.operation_code_17,
                        oc_17_detail = workCenter.Where(w => w.wc.Trim() == routing.operation_code_17.Trim()).Select(s => s.wc_name).FirstOrDefault(),
                        img_oc_17 = isExistsPath(routing.operation_code_17),
                        oc_18 = routing.operation_code_18,
                        oc_18_detail = workCenter.Where(w => w.wc.Trim() == routing.operation_code_18.Trim()).Select(s => s.wc_name).FirstOrDefault(),
                        img_oc_18 = isExistsPath(routing.operation_code_18),
                        oc_19 = routing.operation_code_19,
                        oc_19_detail = workCenter.Where(w => w.wc.Trim() == routing.operation_code_19.Trim()).Select(s => s.wc_name).FirstOrDefault(),
                        img_oc_19 = isExistsPath(routing.operation_code_19)
                    };

                var job_progressObj = new
                    {
                        wc_00 = job_progress != null ? job_progress.wc_00 : null,
                        wc_00_detail = job_progress != null ? workCenter.Where(w => w.wc.Trim() == (job_progress.wc_00 == null ? "" : job_progress.wc_00.Trim())).Select(s => s.wc_name).FirstOrDefault() : null,
                        wc_00_dt = job_progress != null ? getWCDateTime(id, job_progress.wc_00) : "<span style='color:red'>Job Progress Not Found!!</span>",
                        img_wc_00 = job_progress != null ? isExistsPath(job_progress.wc_00) : null,
                        wc_01 = job_progress != null ? job_progress.wc_01 : null,
                        wc_01_detail = job_progress != null ? workCenter.Where(w => w.wc.Trim() == (job_progress.wc_01 == null ? "" : job_progress.wc_01.Trim())).Select(s => s.wc_name).FirstOrDefault() : null,
                        wc_01_dt = job_progress != null ? getWCDateTime(id, job_progress.wc_01) : "<span style='color:red'>Job Progress Not Found!!</span>",
                        img_wc_01 = job_progress != null ? isExistsPath(job_progress.wc_01) : null,
                        wc_02 = job_progress != null ? job_progress.wc_02 : null,
                        wc_02_detail = job_progress != null ? workCenter.Where(w => w.wc.Trim() == (job_progress.wc_02 == null ? "" : job_progress.wc_02.Trim())).Select(s => s.wc_name).FirstOrDefault() : null,
                        wc_02_dt = job_progress != null ? getWCDateTime(id, job_progress.wc_02) : "<span style='color:red'>Job Progress Job Progress Not Found!!</span>",
                        img_wc_02 = job_progress != null ? isExistsPath(job_progress.wc_02) : null,
                        wc_03 = job_progress != null ? job_progress.wc_03 : null,
                        wc_03_detail = job_progress != null ? workCenter.Where(w => w.wc.Trim() == (job_progress.wc_03 == null ? "" : job_progress.wc_03.Trim())).Select(s => s.wc_name).FirstOrDefault() : null,
                        wc_03_dt = job_progress != null ? getWCDateTime(id, job_progress.wc_03) : "<span style='color:red'>Job Progress Not Found!!</span>",
                        img_wc_03 = job_progress != null ? isExistsPath(job_progress.wc_03) : null,
                        wc_04 = job_progress != null ? job_progress.wc_04 : null,
                        wc_04_detail = job_progress != null ? workCenter.Where(w => w.wc.Trim() == (job_progress.wc_04 == null ? "" : job_progress.wc_04.Trim())).Select(s => s.wc_name).FirstOrDefault() : null,
                        wc_04_dt = job_progress != null ? getWCDateTime(id, job_progress.wc_04) : "<span style='color:red'>Job Progress Not Found!!</span>",
                        img_wc_04 = job_progress != null ? isExistsPath(job_progress.wc_04) : null,
                        wc_05 = job_progress != null ? job_progress.wc_05 : null,
                        wc_05_detail = job_progress != null ? workCenter.Where(w => w.wc.Trim() == (job_progress.wc_05 == null ? "" : job_progress.wc_05.Trim())).Select(s => s.wc_name).FirstOrDefault() : null,
                        wc_05_dt = job_progress != null ? getWCDateTime(id, job_progress.wc_05) : "<span style='color:red'>Job Progress Not Found!!</span>",
                        img_wc_05 = job_progress != null ? isExistsPath(job_progress.wc_05) : null,
                        wc_06 = job_progress != null ? job_progress.wc_06 : null,
                        wc_06_detail = job_progress != null ? workCenter.Where(w => w.wc.Trim() == (job_progress.wc_06 == null ? "" : job_progress.wc_06.Trim())).Select(s => s.wc_name).FirstOrDefault() : null,
                        wc_06_dt = job_progress != null ? getWCDateTime(id, job_progress.wc_06) : "<span style='color:red'>Job Progress Not Found!!</span>",
                        img_wc_06 = job_progress != null ? isExistsPath(job_progress.wc_06) : null,
                        wc_07 = job_progress != null ? job_progress.wc_07 : null,
                        wc_07_detail = job_progress != null ? workCenter.Where(w => w.wc.Trim() == (job_progress.wc_07 == null ? "" : job_progress.wc_07.Trim())).Select(s => s.wc_name).FirstOrDefault() : null,
                        wc_07_dt = job_progress != null ? getWCDateTime(id, job_progress.wc_07) : "<span style='color:red'>Job Progress Not Found!!</span>",
                        img_wc_07 = job_progress != null ? isExistsPath(job_progress.wc_07) : null,
                        wc_08 = job_progress != null ? job_progress.wc_08 : null,
                        wc_08_detail = job_progress != null ? workCenter.Where(w => w.wc.Trim() == (job_progress.wc_08 == null ? "" : job_progress.wc_08.Trim())).Select(s => s.wc_name).FirstOrDefault() : null,
                        wc_08_dt = job_progress != null ? getWCDateTime(id, job_progress.wc_08) : "<span style='color:red'>Job Progress Not Found!!</span>",
                        img_wc_08 = job_progress != null ? isExistsPath(job_progress.wc_08) : null,
                        wc_09 = job_progress != null ? job_progress.wc_09 : null,
                        wc_09_detail = job_progress != null ? workCenter.Where(w => w.wc.Trim() == (job_progress.wc_09 == null ? "" : job_progress.wc_09.Trim())).Select(s => s.wc_name).FirstOrDefault() : null,
                        wc_09_dt = job_progress != null ? getWCDateTime(id, job_progress.wc_09) : "<span style='color:red'>Job Progress Not Found!!</span>",
                        img_wc_09 = job_progress != null ? isExistsPath(job_progress.wc_09) : null,
                        wc_10 = job_progress != null ? job_progress.wc_10 : null,
                        wc_10_detail = job_progress != null ? workCenter.Where(w => w.wc.Trim() == (job_progress.wc_10 == null ? "" : job_progress.wc_10.Trim())).Select(s => s.wc_name).FirstOrDefault() : null,
                        wc_10_dt = job_progress != null ? getWCDateTime(id, job_progress.wc_10) : "<span style='color:red'>Job Progress Not Found!!</span>",
                        img_wc_10 = job_progress != null ? isExistsPath(job_progress.wc_10) : null,
                        wc_11 = job_progress != null ? job_progress.wc_11 : null,
                        wc_11_detail = job_progress != null ? workCenter.Where(w => w.wc.Trim() == (job_progress.wc_11 == null ? "" : job_progress.wc_11.Trim())).Select(s => s.wc_name).FirstOrDefault() : null,
                        wc_11_dt = job_progress != null ? getWCDateTime(id, job_progress.wc_11) : "<span style='color:red'>Job Progress Not Found!!</span>",
                        img_wc_11 = job_progress != null ? isExistsPath(job_progress.wc_11) : null,
                        wc_12 = job_progress != null ? job_progress.wc_12 : null,
                        wc_12_detail = job_progress != null ? workCenter.Where(w => w.wc.Trim() == (job_progress.wc_12 == null ? "" : job_progress.wc_12.Trim())).Select(s => s.wc_name).FirstOrDefault() : null,
                        wc_12_dt = job_progress != null ? getWCDateTime(id, job_progress.wc_12) : "<span style='color:red'>Job Progress Not Found!!</span>",
                        img_wc_12 = job_progress != null ? isExistsPath(job_progress.wc_12) : null,
                        wc_13 = job_progress != null ? job_progress.wc_13 : null,
                        wc_13_detail = job_progress != null ? workCenter.Where(w => w.wc.Trim() == (job_progress.wc_13 == null ? "" : job_progress.wc_13.Trim())).Select(s => s.wc_name).FirstOrDefault() : null,
                        wc_13_dt = job_progress != null ? getWCDateTime(id, job_progress.wc_13) : "<span style='color:red'>Job Progress Not Found!!</span>",
                        img_wc_13 = job_progress != null ? isExistsPath(job_progress.wc_13) : null,
                        wc_14 = job_progress != null ? job_progress.wc_14 : null,
                        wc_14_detail = job_progress != null ? workCenter.Where(w => w.wc.Trim() == (job_progress.wc_14 == null ? "" : job_progress.wc_14.Trim())).Select(s => s.wc_name).FirstOrDefault() : null,
                        wc_14_dt = job_progress != null ? getWCDateTime(id, job_progress.wc_14) : "<span style='color:red'>Job Progress Not Found!!</span>",
                        img_wc_14 = job_progress != null ? isExistsPath(job_progress.wc_14) : null,
                        wc_15 = job_progress != null ? job_progress.wc_15 : null,
                        wc_15_detail = job_progress != null ? workCenter.Where(w => w.wc.Trim() == (job_progress.wc_15 == null ? "" : job_progress.wc_15.Trim())).Select(s => s.wc_name).FirstOrDefault() : null,
                        wc_15_dt = job_progress != null ? getWCDateTime(id, job_progress.wc_15) : "<span style='color:red'>Job Progress Not Found!!</span>",
                        img_wc_15 = job_progress != null ? isExistsPath(job_progress.wc_15) : null,
                        wc_16 = job_progress != null ? job_progress.wc_16 : null,
                        wc_16_detail = job_progress != null ? workCenter.Where(w => w.wc.Trim() == (job_progress.wc_16 == null ? "" : job_progress.wc_16.Trim())).Select(s => s.wc_name).FirstOrDefault() : null,
                        wc_16_dt = job_progress != null ? getWCDateTime(id, job_progress.wc_16) : "<span style='color:red'>Job Progress Not Found!!</span>",
                        img_wc_16 = job_progress != null ? isExistsPath(job_progress.wc_16) : null,
                        wc_17 = job_progress != null ? job_progress.wc_17 : null,
                        wc_17_detail = job_progress != null ? workCenter.Where(w => w.wc.Trim() == (job_progress.wc_17 == null ? "" : job_progress.wc_17.Trim())).Select(s => s.wc_name).FirstOrDefault() : null,
                        wc_17_dt = job_progress != null ? getWCDateTime(id, job_progress.wc_17) : "<span style='color:red'>Job Progress Not Found!!</span>",
                        img_wc_17 = job_progress != null ? isExistsPath(job_progress.wc_17) : null,
                        wc_18 = job_progress != null ? job_progress.wc_18 : null,
                        wc_18_detail = job_progress != null ? workCenter.Where(w => w.wc.Trim() == (job_progress.wc_18 == null ? "" : job_progress.wc_18.Trim())).Select(s => s.wc_name).FirstOrDefault() : null,
                        wc_18_dt = job_progress != null ? getWCDateTime(id, job_progress.wc_18) : "<span style='color:red'>Job Progress Not Found!!</span>",
                        img_wc_18 = job_progress != null ? isExistsPath(job_progress.wc_18) : null,
                        wc_19 = job_progress != null ? job_progress.wc_19 : null,
                        wc_19_detail = job_progress != null ? workCenter.Where(w => w.wc.Trim() == (job_progress.wc_19 == null ? "" : job_progress.wc_19.Trim())).Select(s => s.wc_name).FirstOrDefault() : null,
                        wc_19_dt = job_progress != null ? getWCDateTime(id, job_progress.wc_19) : "<span style='color:red'>Job Progress Not Found!!</span>",
                        img_wc_19 = job_progress != null ? isExistsPath(job_progress.wc_19) : null
                    };

                var next_progressObj = new
                    {
                        item = next_progress != null ? next_progress.item : null,
                        job_order_no = next_progress != null ? next_progress.job_order_no : null,
                        stat = next_progress != null ? next_progress.stat : null,
                        wc = next_progress != null ? next_progress.wc : null,
                        img_wc = next_progress != null ? isExistsPath(next_progress.wc) : null,
                        wc_detail = next_progress != null ? workCenter.Where(w => w.wc.Trim() == next_progress.wc.Trim()).Select(s => s.wc_name).FirstOrDefault() : null
                    };

                var deliveryObj = new
                    {
                        print_status = delivery != null ? delivery.print_status : null,
                        print_date = delivery != null && delivery.print_date != null ? convDateText(delivery.print_date.ToString(), delivery.print_time) : null,
                        print_user = delivery != null ? formatName(delivery.print_user) : null,
                        deliver_note = delivery != null ? delivery.delivery_no : null,
                        loading_status = delivery != null ? delivery.loading_status : null,
                        loading_date = delivery != null && delivery.loading_date != null ? convDateText(delivery.loading_date.ToString(), delivery.loading_time) : null,
                        loading_user = delivery != null ? formatName(delivery.loading_user) : null,
                        receiving_status = delivery != null ? delivery.receiving_status : null,
                        receiving_date = delivery != null && delivery.receiving_date != null ? convDateText(delivery.receiving_date == null ? null : delivery.receiving_date.ToString(), delivery.receiving_time) : null,
                        receiving_user = delivery != null ? formatName(delivery.receiving_user) : null
                    };

                jsondata = new
                {
                    routing = routingObj, job_progress = job_progressObj, next_progress = next_progressObj, delivery = deliveryObj
                };
            }
            catch (Exception)
            {
                throw;
            }
            
            return Json(jsondata, JsonRequestBehavior.AllowGet);
        }

        public string getWCDateTime(string job, string wc)
        {
            var partJobNo = job.Trim().Substring(0, (job.Length - 1));
            var dt = db.td_part_recdata.Where(w => w.job_order_no.Contains(partJobNo) && (w.wc != null && w.wc == wc)).OrderByDescending(o => o.update_date).OrderByDescending(o => o.update_time).FirstOrDefault();
            if (dt != null)
                return util.convDateIntToStr(dt.insert_date) + " " + dt.insert_time + "<br />User: " + formatName(dt.entry_user);
            else
                return "<span style='color:red'>Not Found!!</span>";
        }

        public string isExistsPath(string wc)
        {
            if (wc != null)
            {
                string originalImg = wc.Trim() + ".JPG",
                    defaultImg = "Default.JPG";

                if (System.IO.File.Exists(Server.MapPath("~/Images/WC_Images/" + originalImg)))
                {
                    return originalImg;
                }
                else
                {
                    return defaultImg;
                }
            }
            else
                return string.Empty;
        }

        public ActionResult Report()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Report(string selType, string txtDateFrom, string txtDateTo, string radGrouping,
            string WCPart, string WCFG)
        {
            //var grid = new System.Web.UI.WebControls.GridView();

            //var tmpQ = from r in db.td_part_result
            //           join d in db.td_part_recdata
            //           on r.part_job_order_no equals d.job_order_no
            //           select new
            //           {
            //               result = r,
            //               recdata = d
            //           };

            //if (!string.IsNullOrEmpty(txtDateFrom))
            //{
            //    var dfrom = convDate(txtDateFrom);
            //    tmpQ = tmpQ.Where(w => w.result.result_date >= dfrom);
            //}

            //if (!string.IsNullOrEmpty(txtDateTo))
            //{
            //    var dto = convDate(txtDateTo);
            //    tmpQ = tmpQ.Where(w => w.result.result_date <= dto);
            //}

            //if (selType == "ProductionResult")
            //{
            //    if (radGrouping == "Grouping")
            //    {
            //        grid.DataSource = (from d in tmpQ
            //                           group d by d.recdata.job_order_no into g
            //                           select new
            //                           {
            //                               jobNo = g.Key,
            //                               tagNo = "'" + g.Min(m => m.recdata.tag_no),
            //                               lineNo = g.Min(m => m.recdata.wc_cure) + (g.Min(m => m.recdata.wc_cure) != "" && g.Min(m => m.recdata.wc_cure) != null ? "/" : "") + g.Min(m => m.recdata.mc_cure),
            //                               productCode = g.Min(m => m.recdata.part_item),
            //                               lotNo = g.Min(m => m.recdata.qty_tag),
            //                               wc_fg = g.Min(m => m.result.wc),
            //                               date = g.Min(m => m.result.result_date),
            //                               time = g.Min(m => m.result.result_time),
            //                               user = g.Min(m => m.result.result_user)
            //                           }).ToList()
            //                           .Select(g => new
            //                           {
            //                               jobNo = g.jobNo,
            //                               tagNo = g.tagNo,
            //                               lineNo = g.lineNo,
            //                               productCode = g.productCode,
            //                               lotNo = g.lotNo,
            //                               wc_fg = g.wc_fg,
            //                               result_date = g.date.Value.ToShortDateString(),
            //                               result_time = g.time.Value.ToString(),
            //                               result_user = g.user
            //                           });
            //    }
            //    else
            //    {
            //        grid.DataSource = tmpQ.ToList().Select(d => new
            //        {
            //            jobNo = d.recdata.job_order_no,
            //            tagNo = "'" + d.recdata.tag_no,
            //            lineNo = d.recdata.wc_cure + (d.recdata.wc_cure != "" && d.recdata.wc_cure != null ? "/" : "") + d.recdata.mc_cure,
            //            productCode = d.recdata.part_item,
            //            lotNo = d.recdata.qty_tag,
            //            basketNo = d.recdata.basket_num,
            //            wc_part = d.recdata.wc,
            //            wc_fg = d.result.wc,
            //            result_date = d.result.result_date.Value.ToShortDateString(),
            //            result_time = d.result.result_time.Value.ToString(),
            //            result_user = d.result.result_user
            //        });
            //    }
            //}
            //else if (selType == "ResultProcess")
            //{
            //    if (!string.IsNullOrEmpty(WCPart))
            //    {
            //        tmpQ = tmpQ.Where(w => w.recdata.wc.Trim() == WCPart.Trim());
            //    }

            //    if (!string.IsNullOrEmpty(WCFG))
            //    {
            //        tmpQ = tmpQ.Where(w => w.result.wc.Trim() == WCFG.Trim());
            //    }

            //    grid.DataSource = tmpQ.ToList().Select(d => new
            //    {
            //        jobNo = d.recdata.job_order_no,
            //        tagNo = "'" + d.recdata.tag_no,
            //        lineNo = d.recdata.wc_cure + (d.recdata.wc_cure != "" && d.recdata.wc_cure != null ? "/" : "") + d.recdata.mc_cure,
            //        productCode = d.recdata.part_item,
            //        lotNo = d.recdata.qty_tag,
            //        basketNo = d.recdata.basket_num,
            //        wc_part = d.recdata.wc,
            //        wc_fg = d.result.wc,
            //        result_date = d.result.result_date.Value.ToShortDateString(),
            //        result_time = d.result.result_time.Value.ToString(),
            //        result_user = d.result.result_user
            //    }); ;
            //}
            //else
            //    throw new Exception();

            //if (!tmpQ.Any())
            //{
            //    ViewBag.Message = "No data found!";
            //}
            //else
            //{
            //    ViewBag.Message = "";

            //    grid.DataBind();

            //    Response.ClearContent();
            //    Response.AddHeader("content-disposition", "attachment; filename=ProductionResult.xls");
            //    Response.ContentType = "application/excel";
            //    StringWriter sw = new StringWriter();
            //    HtmlTextWriter htw = new HtmlTextWriter(sw);
            //    grid.RenderControl(htw);
            //    Response.Write(sw.ToString());
            //    Response.End();
            //}
            

            return View();
        }

        public ActionResult GetPartjobLink(string parentJob)
        {
            var metal = qim.tr_metal_job_nics.Where(w => w.job_order_no.Trim() == parentJob).Select(s => s.parts_job_order_no).ToList();
            var part = qim.tr_part_nics.Where(w => w.job_order_no.Trim() == parentJob).Select(s => s.parts_job_order_no).ToList();

            var union = metal.Union(part);
            return Json(union, JsonRequestBehavior.AllowGet);
        }

        public ActionResult About()
        {
            //ViewBag.Message = "Your app description page.";
            return View();
        }

        public ActionResult Contact()
        {
            //ViewBag.Message = "Your contact page.";
            return View();
        }
    }
}
