using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataCollection.Models;
using MvcJqGrid;
using System.Linq.Dynamic;
using System.Data.Objects.SqlClient;
using WebCommonFunction;

namespace DataCollection.Controllers
{
    public class ReportController : Controller
    {
        partnicsEntities parts = new partnicsEntities();
        ekanbannicsEntities kanban = new ekanbannicsEntities();
        rbstocknicsEntities rbstock = new rbstocknicsEntities();
        tncproductionliveEntities tncProd = new tncproductionliveEntities();
        qimnicsEntities qim = new qimnicsEntities();
        private TNCUtility ut = new TNCUtility();

        Util util = new Util();

        //
        // GET: /Report/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /SBGroup/

        public ActionResult SBGroup(string id)
        {
            ViewBag.Title = id;
            return View();
        }

        //
        // GET: /SBNGroup/

        public ActionResult SBNGroup(string id)
        {
            ViewBag.Title = id;
            return View();
        }

        //
        // GET: /SumDeliverSheet/

        public ActionResult SumDeliverSheet()
        {
            ViewBag.Title = "Summary Delivery Sheet";
            return View();
        }

        //
        // GET: /ResultByMachine/

        public ActionResult ResultByMachine()
        {
            ViewBag.Title = "Production Result by Machine";
            return View();
        }

        //
        // GET: /WaitForReceive/

        public ActionResult WaitForReceive()
        {
            ViewBag.Title = "Wait For Production Receive";
            return View();
        }

        public ActionResult RubberDailySupplyKanban()
        {
            ViewBag.Title = "Rubber Daily Supply Kanban";
            return View();
        }

        //
        // GET: /GetSBNGroupJSON/

        public ActionResult GetSBNGroupJSON(GridSettings gridSettings, string datefrom, string dateto, string timefrom, string timeto, string fgcode, byte stype)
        {
            var data = (from r in parts.td_part_result
                        join w in parts.tm_metal_weight
                        on r.finished_goods_code.Substring(0, r.finished_goods_code.Length-3) equals w.finished_goods_code into j
                        from x in j.DefaultIfEmpty()
                        select new
                        {
                            result = r,
                            weight = x
                        }).OrderBy(gridSettings.SortColumn + " " + gridSettings.SortOrder);

            // Filter Condition

            if (!string.IsNullOrEmpty(datefrom))
            {
                var dfrom = util.convDateStrToInt(datefrom);
                data = data.Where(w => w.result.result_date >= dfrom);
            }

            if (!string.IsNullOrEmpty(dateto))
            {
                var dto = util.convDateStrToInt(dateto);
                data = data.Where(w => w.result.result_date <= dto);
            }


            //bool timeflag = false;
            //if (!string.IsNullOrEmpty(timefrom) && !string.IsNullOrEmpty(timeto))
            //    timeflag = true;

            //if (!string.IsNullOrEmpty(datefrom))
            //{
            //    var dfrom = util.convDateStrToInt(datefrom);
            //    if (timeflag)
            //    {
            //        var time = TimeSpan.Parse(util.convInputTimeDispFormat(timefrom)).ToString();
            //        data = data.Where(w => (w.entry_date.Value.ToString() + w.entry_time.Value).CompareTo((dfrom.ToString() + time)) >= 0);
            //    }
            //    else
            //        data = data.Where(w => w.entry_date >= dfrom);
            //}

            //if (!string.IsNullOrEmpty(dateto))
            //{
            //    var dto = util.convDateStrToDBFormatStr(sdateto);
            //    data = data.Where(w => w.supply_date.CompareTo(dto) <= 0);
            //    if (timeflag)
            //    {
            //        var timeto = util.convTimeSQLFormat(stimeto);
            //        data = data.Where(w => (w.supply_date + w.supply_time).CompareTo((dto + timeto)) <= 0);
            //    }
            //    else
            //        data = data.Where(w => w.supply_date.CompareTo(dto) <= 0);
            //}



            if (!string.IsNullOrEmpty(stype.ToString()))
            {
                if (stype == 1)
                {
                    data = data.Where(w => (w.result.result_type.Trim() == "1") && (w.result.process_result.Trim() == "1" || w.result.process_result.Trim() == "2"));
                }
                else
                {
                    data = data.Where(w => w.result.result_type.Trim() == "2" && w.result.process_result.Trim() == "2");
                }
            }

            if (!string.IsNullOrEmpty(fgcode))
            {
                data = data.Where(w => w.result.finished_goods_code.Contains(fgcode));
            }

            // Count All Record
            double count = data.Count();

            // Select Data Per Page
            var dataTmp = from d in data.Skip((gridSettings.PageIndex - 1) * gridSettings.PageSize).Take(gridSettings.PageSize).ToList()
                          select new
                          {
                              entry_date = util.convDateIntToStr(d.result.entry_date),
                              entry_time = d.result.entry_time,
                              result_date = util.convDateIntToStr(d.result.result_date),
                              result_time = d.result.result_time,
                              d.result.wc,
                              d.result.finished_goods_code,
                              d.result.parent_job_order_no,
                              d.result.part_code,
                              d.result.part_job_order_no,
                              d.result.qty,
                              weight = d.weight != null ? d.weight.weight : 0,
                              totalweight = d.weight != null ? d.result.qty * d.weight.weight : 0,
                              d.result.entry_user
                          };

            var selectData = dataTmp.Select(d => new
            {
                id = d.part_job_order_no,
                cell = new object[]
                                        {
                                            d.entry_date,
                                            d.entry_time,
                                            d.result_date,
                                            d.result_time,
                                            d.wc,
                                            d.finished_goods_code,
                                            d.parent_job_order_no,
                                            d.part_code,
                                            d.part_job_order_no,
                                            d.qty,
                                            d.weight,
                                            d.totalweight,
                                            d.entry_user
                                        }
            });
            
            // Prepare JSON Format
            object jsonData = new
            {
                page = gridSettings.PageIndex,
                total = Math.Ceiling(count / gridSettings.PageSize),
                records = count,
                rows = selectData
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /GetSBGroupJSON/

        public ActionResult GetSBGroupJSON(GridSettings gridSettings, string datefrom, string dateto, string fgcode, byte stype)
        {
            var dfrom = util.convDateStrToInt(datefrom);
            var dto = util.convDateStrToInt(dateto);

            var data = (from  r in (from p in parts.td_part_result
                        where p.entry_date >= dfrom && p.entry_date <= dto
                        group p by p.parent_job_order_no into g
                        
                        select new
                        {
                            //result_date = g.Min(m => m.result_date),
                            //result_time = g.Min(m => m.result_time),
                            wc = g.Max(m => m.wc),
                            finished_goods_code = g.Max(m => m.finished_goods_code),
                            parent_job_order_no = g.Key,
                            qty = g.Max(m => m.qty),
                            minType = g.Min(m => m.result_type.Trim()),
                            maxType = g.Max(m => m.result_type.Trim()),
                            minProc = g.Min(m => m.process_result.Trim()),
                            maxProc = g.Max(m => m.process_result.Trim())
                        })
                        join w in parts.tm_metal_weight
                        on r.finished_goods_code.Substring(0, r.finished_goods_code.Length - 3) equals w.finished_goods_code into j
                        from x in j.DefaultIfEmpty()
                        select new {
                            result = r,
                            weight = x
                        })
                        .OrderBy(gridSettings.SortColumn + " " + gridSettings.SortOrder);

            // Filter Condition
            if (!string.IsNullOrEmpty(stype.ToString()))
            {
                if (stype == 1)
                {
                    data = data.Where(w => (w.result.minType == "1") && (w.result.minProc == "1" || w.result.maxProc == "2"));
                }
                else
                {
                    data = data.Where(w => w.result.maxType == "2" && w.result.maxProc == "2");
                }
            }

            if (!string.IsNullOrEmpty(fgcode))
            {
                data = data.Where(w => w.result.finished_goods_code.Contains(fgcode));
            }

            // Count All Record
            double count = data.Count();

            // Select Data Per Page
            var dataTmp = from d in data.Skip((gridSettings.PageIndex - 1) * gridSettings.PageSize).Take(gridSettings.PageSize).ToList()
                          select new
                          {
                              //result_date = d.result.result_date,
                              //result_time = d.result.result_time,
                              d.result.wc,
                              d.result.finished_goods_code,
                              d.result.parent_job_order_no,
                              d.result.qty,
                              weight = d.weight != null ? d.weight.weight : 0,
                              totalweight = d.weight != null ? d.result.qty * d.weight.weight : 0,
                          };

            var selectData = dataTmp.Select(d => new
            {
                id = d.parent_job_order_no,
                cell = new object[]
                                        {
                                            //util.convDateIntToStr(d.result_date),
                                            //d.result_time.Value.ToString(),
                                            d.wc,
                                            d.finished_goods_code,
                                            d.parent_job_order_no,
                                            d.qty,
                                            d.weight,
                                            d.totalweight
                                        }
            });

            // Prepare JSON Format
            object jsonData = new
            {
                page = gridSettings.PageIndex,
                total = Math.Ceiling(count / gridSettings.PageSize),
                records = count,
                rows = selectData
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /GetSumDeliverSheet/

        public ActionResult GetSumDeliverSheet(GridSettings gridSettings, string datefrom, string dateto, string deliverNo
            ,string incdatefrom, string incdateto , string plant, string cartNo, string truckNo, string jobNo)
        {

            var data = (parts.td_part_delivery).Where(w => w.loading_status == "1");

            // Filter Condition
            if (!string.IsNullOrEmpty(datefrom))
            {
                var dfrom = util.convDateStrToDBFormatStr(datefrom);
                data = data.Where(w => (w.loading_date + w.loading_time).CompareTo(dfrom) >= 0);
            }

            if (!string.IsNullOrEmpty(dateto))
            {
                var dto = util.convDateStrToDBFormatStr(dateto);
                data = data.Where(w => (w.loading_date + w.loading_time).CompareTo(dto) <= 0);
            }

            if (!string.IsNullOrEmpty(incdatefrom))
            {
                var dfrom = util.convDateStrToDBFormatStr(incdatefrom);
                data = data.Where(w => w.incoming_date.CompareTo(dfrom) >= 0);
            }

            if (!string.IsNullOrEmpty(incdateto))
            {
                var dto = util.convDateStrToDBFormatStr(incdateto);
                data = data.Where(w => w.incoming_date.CompareTo(dto) <= 0);
            }

            if (!string.IsNullOrEmpty(deliverNo))
            {
                data = data.Where(w => w.delivery_no.Contains(deliverNo));
            }

            if (!string.IsNullOrEmpty(plant))
            {
                data = data.Where(w => w.plant.Contains(plant));
            }

            if (!string.IsNullOrEmpty(cartNo))
            {
                data = data.Where(w => w.cart_no.Contains(cartNo));
            }

            if (!string.IsNullOrEmpty(truckNo))
            {
                data = data.Where(w => w.truck_no.Contains(truckNo));
            }

            if (!string.IsNullOrEmpty(jobNo))
            {
                data = data.Where(w => w.part_job_order_no.Contains(jobNo));
            }

            // Count All Record
            double count = data.Count();

            // Select Data Per Page
            var selectData = data.OrderBy(gridSettings.SortColumn + " " + gridSettings.SortOrder).Skip((gridSettings.PageIndex - 1) * gridSettings.PageSize).Take(gridSettings.PageSize).ToList().Select(d => new
            {
                id = d.part_job_order_no,
                cell = new object[]
                                        {
                                            d.loading_user,
                                            util.convDateIntToStr(d.loading_date),
                                            util.convTimeStrFormat(d.loading_time),
                                            d.working_shift,
                                            d.wc,
                                            d.finished_goods_code,
                                            d.part_code,
                                            d.delivery_no,
                                            d.plant,
                                            d.part_job_order_no,
                                            d.tag_no,
                                            d.qty
                                        }
            });

            // Prepare JSON Format
            object jsonData = new
            {
                page = gridSettings.PageIndex,
                total = Math.Ceiling(count / gridSettings.PageSize),
                records = count,
                rows = selectData
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /GetWaitForReceive/

        public ActionResult GetWaitForReceive(GridSettings gridSettings, string datefrom, string dateto)
        {
            var data = (parts.td_part_delivery).Where(w => w.loading_status == "1" && w.receiving_status != "1").OrderBy(gridSettings.SortColumn + " " + gridSettings.SortOrder);
            // Filter Condition
            if (!string.IsNullOrEmpty(datefrom))
            {
                var dfrom = util.convDateStrToDBFormatStr(datefrom);
                data = data.Where(w => w.loading_date.CompareTo(dfrom) >= 0);
            }

            if (!string.IsNullOrEmpty(dateto))
            {
                var dto = util.convDateStrToDBFormatStr(dateto);
                data = data.Where(w => w.loading_date.CompareTo(dto) <= 0);
            }

            // Count All Record
            double count = data.Count();

            // Select Data Per Page
            var dataTmp = from d in data.Skip((gridSettings.PageIndex - 1) * gridSettings.PageSize).Take(gridSettings.PageSize).ToList()
                          select new
                          {
                              d.loading_user,
                              loading_date = util.convDateIntToStr(d.loading_date),
                              loading_time = util.convTimeStrFormat(d.loading_time),
                              d.working_shift,
                              d.delivery_no,
                              d.plant,
                              d.cart_no,
                              d.truck_no,
                              d.wc,
                              d.finished_goods_code,
                              d.part_code,
                              d.part_job_order_no,
                              d.tag_no,
                              d.qty
                          };

            var selectData = dataTmp.Select(d => new
            {
                id = d.part_job_order_no,
                cell = new object[]
                                        {
                                            d.loading_user,
                                            d.loading_date,
                                            d.loading_time,
                                            d.working_shift,
                                            d.wc,
                                            d.finished_goods_code,
                                            d.part_code,
                                            d.delivery_no,
                                            d.plant,
                                            d.part_job_order_no,
                                            d.tag_no,
                                            d.qty
                                        }
            });

            // Prepare JSON Format
            object jsonData = new
            {
                page = gridSettings.PageIndex,
                total = Math.Ceiling(count / gridSettings.PageSize),
                records = count,
                rows = selectData
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /GetWaitForReceive/

        public ActionResult GetRubberDailySupplyKanban(GridSettings gridSettings, string compound, string wc, string jobno, string odatefrom, string odateto, string sdatefrom, string sdateto)
        {
            var data = (from d in rbstock.td_job_information
                        where d.group_cd.Trim() == "DC" || d.group_cd.Trim() == "CBS" || d.group_cd.Trim() == "SNT"
                         || d.group_cd.Trim() == "INJ" || d.group_cd.Trim() == "RBB" || d.group_cd.Trim() == "BUSH"
                        select d).OrderBy(gridSettings.SortColumn + " " + gridSettings.SortOrder);

            // Filter Condition
            if (!string.IsNullOrEmpty(compound))
            {
                data = data.Where(w => w.compound.Contains(compound));
            }

            if (!string.IsNullOrEmpty(wc))
            {
                data = data.Where(w => w.wc.Contains(wc));
            }

            if (!string.IsNullOrEmpty(jobno))
            {
                data = data.Where(w => w.job_order_no == jobno);
            }

            if (!string.IsNullOrEmpty(odatefrom))
            {
                var dfrom = util.convDateTimeStrToDBFormatStr(odatefrom);
                data = data.Where(w => (w.takeout_date + w.takeout_time).CompareTo(dfrom) >= 0);
            }

            if (!string.IsNullOrEmpty(odateto))
            {
                var dto = util.convDateTimeStrToDBFormatStr(odateto);
                data = data.Where(w => (w.takeout_date + w.takeout_time).CompareTo(dto) <= 0);
            }

            if (!string.IsNullOrEmpty(sdatefrom))
            {
                var dfrom = util.convDateTimeStrToDBFormatStr(sdatefrom);
                data = data.Where(w => (w.supply_date + w.supply_time).CompareTo(dfrom) >= 0);
            }

            if (!string.IsNullOrEmpty(sdateto))
            {
                var dto = util.convDateTimeStrToDBFormatStr(sdateto);
                data = data.Where(w => (w.supply_date + w.supply_time).CompareTo(dto) <= 0);
            }

            // Count All Record
            double count = data.Count();

            // Select Data Per Page
            var selectData = data.Skip((gridSettings.PageIndex - 1) * gridSettings.PageSize).Take(gridSettings.PageSize).ToList().Select(d => new
            {
                id = d.job_order_no,
                cell = new object[]
                {
                    d.supply_box,
                    d.compound,
                    d.kanban_no,
                    d.weight,
                    util.convDateIntToStr(d.supply_date) + " " + util.convTimeStrFormat(d.supply_time),
                    util.convUserNameFormat(d.supply_user),
                    d.wc,
                    d.item,
                    d.job_order_no,
                    d.tag_no,
                    util.convDateIntToStr(d.takeout_date) + " " + util.convTimeStrFormat(d.takeout_time),
                    util.convUserNameFormat(d.order_user)
                }
            });



            // Prepare JSON Format
            object jsonData = new
            {
                page = gridSettings.PageIndex,
                total = Math.Ceiling(count / gridSettings.PageSize),
                records = count,
                rows = selectData
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        
        // GET: /GetSBNGroupJSON/

        public ActionResult GetResultByMachine(GridSettings gridSettings, string datefrom, string dateto, string wcfg)
        {
            var data = (parts.td_part_result).OrderBy(gridSettings.SortColumn + " " + gridSettings.SortOrder);
            // Filter Condition
            if (!string.IsNullOrEmpty(datefrom))
            {
                var dfrom = util.convDateStrToInt(datefrom);
                data = data.Where(w => w.result_date >= dfrom);
            }

            if (!string.IsNullOrEmpty(dateto))
            {
                var dto = util.convDateStrToInt(dateto);
                data = data.Where(w => w.result_date <= dto);
            }

            if (!string.IsNullOrEmpty(wcfg))
            {
                data = data.Where(w => w.wc.Contains(wcfg));
            }

            // Count All Record
            double count = data.Count();

            // Select Data Per Page
            var dataTmp = from d in data.Skip((gridSettings.PageIndex - 1) * gridSettings.PageSize).Take(gridSettings.PageSize).ToList()
                          select new
                          {
                              result_date = util.convDateIntToStr(d.result_date),
                              result_time = d.result_time,
                              d.wc,
                              d.finished_goods_code,
                              d.parent_job_order_no,
                              d.part_code,
                              d.part_job_order_no,
                              d.qty,
                              entry_user = util.convUserNameFormat(d.entry_user)
                          };

            var selectData = dataTmp.Select(d => new
            {
                id = d.part_job_order_no,
                cell = new object[]
                                        {
                                            d.result_date,
                                            d.result_time,
                                            d.wc,
                                            d.finished_goods_code,
                                            d.parent_job_order_no,
                                            d.part_code,
                                            d.part_job_order_no,
                                            d.qty,
                                            d.entry_user
                                        }
            });

            // Prepare JSON Format
            object jsonData = new
            {
                page = gridSettings.PageIndex,
                total = Math.Ceiling(count / gridSettings.PageSize),
                records = count,
                rows = selectData
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ExportToExcelNG(string datefrom, string dateto, string fg, byte stype, string sidx, string sord)
        {
            var gridModel = new OrdersJqGridModelNG();
            var grid = gridModel.OrdersGrid;

            // Prepare data
            var data = (from r in parts.td_part_result
                        join w in parts.tm_metal_weight
                        on r.finished_goods_code equals w.finished_goods_code into j
                        from x in j.DefaultIfEmpty()
                        select new
                        {
                            result = r,
                            weight = x
                        })
                .OrderBy(sidx + " " + sord);
            // Filter Condition
            if (!string.IsNullOrEmpty(datefrom))
            {
                var dfrom = util.convDateStrToInt(datefrom);
                data = data.Where(w => w.result.result_date >= dfrom);
            }

            if (!string.IsNullOrEmpty(dateto))
            {
                var dto = util.convDateStrToInt(dateto);
                data = data.Where(w => w.result.result_date <= dto);
            }

            if (!string.IsNullOrEmpty(stype.ToString()))
            {
                if (stype == 1)
                {
                    data = data.Where(w => (w.result.result_type.Trim() == "1") && (w.result.process_result.Trim() == "1" || w.result.process_result.Trim() == "2"));
                }
                else
                {
                    data = data.Where(w => w.result.result_type.Trim() == "2" && w.result.process_result.Trim() == "2");
                }
            }

            if (!string.IsNullOrEmpty(fg))
            {
                data = data.Where(w => w.result.finished_goods_code.Contains(fg));
            }

            var output = from d in data.ToList()
                         select new
                         {
                             result_date = util.convDateIntToStr(d.result.result_date),
                             d.result.result_time,
                             d.result.wc,
                             d.result.finished_goods_code,
                             d.result.parent_job_order_no,
                             d.result.part_code,
                             d.result.part_job_order_no,
                             d.result.qty,
                             weight = d.weight != null ? d.weight.weight : 0,
                             totalweight = d.weight != null ? d.result.qty * d.weight.weight : 0,
                             d.result.entry_user
                         };

            grid.ExportToExcel(output, "Production_Result_" + (stype == 1 ? "Stamping" : "Bonding") + "_Not_Grouping.xls");

            return View();
        }

        [HttpPost]
        public ActionResult ExportToExcelG(string datefrom, string dateto, string fg, byte stype, string sidx, string sord)
        {
            var gridModel = new OrdersJqGridModelG();
            var grid = gridModel.OrdersGrid;

            // Prepare data
            var dfrom = util.convDateStrToInt(datefrom);
            var dto = util.convDateStrToInt(dateto);

            var data = (from r in
                            (from p in parts.td_part_result
                             where p.entry_date >= dfrom && p.entry_date <= dto
                             group p by p.parent_job_order_no into g

                             select new
                             {
                                 //result_date = g.Min(m => m.result_date),
                                 //result_time = g.Min(m => m.result_time),
                                 wc = g.Max(m => m.wc),
                                 finished_goods_code = g.Max(m => m.finished_goods_code),
                                 parent_job_order_no = g.Key,
                                 qty = g.Max(m => m.qty),
                                 minType = g.Min(m => m.result_type.Trim()),
                                 maxType = g.Max(m => m.result_type.Trim()),
                                 minProc = g.Min(m => m.process_result.Trim()),
                                 maxProc = g.Max(m => m.process_result.Trim())
                             })
                        join w in parts.tm_metal_weight
                        on r.finished_goods_code equals w.finished_goods_code into j
                        from x in j.DefaultIfEmpty()
                        select new
                        {
                            result = r,
                            weight = x
                        })
                        .OrderBy(sidx + " " + sord);

            // Filter Condition
            if (!string.IsNullOrEmpty(stype.ToString()))
            {
                if (stype == 1)
                {
                    data = data.Where(w => (w.result.minType == "1") && (w.result.minProc == "1" || w.result.maxProc == "2"));
                }
                else
                {
                    data = data.Where(w => w.result.maxType == "2" && w.result.maxProc == "2");
                }
            }

            if (!string.IsNullOrEmpty(fg))
            {
                data = data.Where(w => w.result.finished_goods_code.Contains(fg));
            }

            var output = from d in data.ToList()
                         select new
                         {
                             //result_date = util.convDateIntToStr(d.result.result_date),
                             //d.result.result_time,
                             d.result.wc,
                             d.result.finished_goods_code,
                             d.result.parent_job_order_no,
                             d.result.qty,
                             d.result.minType,
                             d.result.maxType,
                             d.result.minProc,
                             d.result.maxProc,
                             weight = d.weight != null ? d.weight.weight : 0,
                             totalweight = d.weight != null ? d.result.qty * d.weight.weight : 0
                         };

            grid.ExportToExcel(output, "Production_Result_" + (stype == 1 ? "Stamping" : "Bonding") + "_Grouping.xls");

            return View();
        }

        [HttpPost]
        public ActionResult ExportToExcelSumDelSheet(string edatefrom, string edateto, string edeliverNo
            , string eincdatefrom, string eincdateto, string eplant, string ecartNo, string etruckNo, string ejobNo, string sidx, string sord)
        {
            var gridModel = new OrdersJqGridModelSumDelSheet();
            var grid = gridModel.OrdersGrid;

            // Prepare data
            var data = (parts.td_part_delivery).Where(w => w.loading_status == "1");

            // Filter Condition
            if (!string.IsNullOrEmpty(edatefrom))
            {
                var dfrom = util.convDateStrToDBFormatStr(edatefrom);
                data = data.Where(w => (w.loading_date + w.loading_time).CompareTo(dfrom) >= 0);
            }

            if (!string.IsNullOrEmpty(edateto))
            {
                var dto = util.convDateStrToDBFormatStr(edateto);
                data = data.Where(w => (w.loading_date + w.loading_time).CompareTo(dto) <= 0);
            }

            if (!string.IsNullOrEmpty(eincdatefrom))
            {
                var dfrom = util.convDateStrToDBFormatStr(eincdatefrom);
                data = data.Where(w => w.incoming_date.CompareTo(dfrom) >= 0);
            }

            if (!string.IsNullOrEmpty(eincdateto))
            {
                var dto = util.convDateStrToDBFormatStr(eincdateto);
                data = data.Where(w => w.incoming_date.CompareTo(dto) <= 0);
            }

            if (!string.IsNullOrEmpty(edeliverNo))
            {
                data = data.Where(w => w.delivery_no.Contains(edeliverNo));
            }

            if (!string.IsNullOrEmpty(eplant))
            {
                data = data.Where(w => w.plant.Contains(eplant));
            }

            if (!string.IsNullOrEmpty(ecartNo))
            {
                data = data.Where(w => w.cart_no.Contains(ecartNo));
            }

            if (!string.IsNullOrEmpty(etruckNo))
            {
                data = data.Where(w => w.truck_no.Contains(etruckNo));
            }

            if (!string.IsNullOrEmpty(ejobNo))
            {
                data = data.Where(w => w.part_job_order_no.Contains(ejobNo));
            }

            // Select Data Per Page
            var output = data.OrderBy(sidx + " " + sord).ToList().Select(d =>
                          new
                          {
                              d.loading_user,
                              loading_date = util.convDateIntToStr(d.loading_date),
                              loading_time = util.convTimeStrFormat(d.loading_time),
                              d.print_user,
                              print_date = util.convDateIntToStr(d.print_date),
                              print_time = util.convTimeStrFormat(d.print_time),
                              incoming_date = util.convDateIntToStr(d.incoming_date),
                              d.working_shift,
                              delivery_no = "'" + d.delivery_no,
                              d.plant,
                              d.cart_no,
                              d.truck_no,
                              d.wc,
                              d.finished_goods_code,
                              d.part_code,
                              d.part_job_order_no,
                              tag_no = "'" + d.tag_no,
                              d.qty
                          });

            grid.ExportToExcel(output, "Summary_Delivery_Sheet.xls");

            return View();
        }

        [HttpPost]
        public ActionResult ExportToExcelResultByMachine(string edatefrom, string edateto, string ewcfg, string sidx, string sord)
        {
            var gridModel = new OrdersJqGridModelResultByMachine();
            var grid = gridModel.OrdersGrid;

            // Prepare data
            var data = (parts.td_part_result).OrderBy(sidx + " " + sord);
            // Filter Condition
            if (!string.IsNullOrEmpty(edatefrom))
            {
                var dfrom = util.convDateStrToInt(edatefrom);
                data = data.Where(w => w.result_date >= dfrom);
            }

            if (!string.IsNullOrEmpty(edateto))
            {
                var dto = util.convDateStrToInt(edateto);
                data = data.Where(w => w.result_date <= dto);
            }

            if (!string.IsNullOrEmpty(ewcfg))
            {
                data = data.Where(w => w.wc.Contains(ewcfg));
            }

            var output = from d in data.ToList()
                         select new
                         {
                             result_date = util.convDateIntToStr(d.result_date),
                             d.result_time,
                             d.wc,
                             d.finished_goods_code,
                             d.parent_job_order_no,
                             d.part_code,
                             d.part_job_order_no,
                             d.qty,
                             d.entry_user
                         };

            grid.ExportToExcel(output, "Production_Result_by_Machine.xls");

            return View();
        }

        [HttpPost]
        public ActionResult ExportToExcelWaitForReceive(string edatefrom, string edateto, string sidx, string sord)
        {
            var gridModel = new OrdersJqGridModelWaitForReceive();
            var grid = gridModel.OrdersGrid;

            // Prepare data
            var data = (parts.td_part_delivery).Where(w => w.loading_status == "1" && w.receiving_status != "1").OrderBy(sidx + " " + sord);

            // Filter Condition
            if (!string.IsNullOrEmpty(edatefrom))
            {
                var dfrom = util.convDateStrToDBFormatStr(edatefrom);
                data = data.Where(w => w.loading_date.CompareTo(dfrom) >= 0);
            }

            if (!string.IsNullOrEmpty(edateto))
            {
                var dto = util.convDateStrToDBFormatStr(edateto);
                data = data.Where(w => w.loading_date.CompareTo(dto) <= 0);
            }

            // Select Data Per Page
            var output = from d in data.ToList()
                         select new
                         {
                             d.loading_user,
                             loading_date = util.convDateIntToStr(d.loading_date),
                             loading_time = util.convTimeStrFormat(d.loading_time),
                             d.working_shift,
                             delivery_no = "'" + d.delivery_no,
                             d.plant,
                             d.cart_no,
                             d.truck_no,
                             d.wc,
                             d.finished_goods_code,
                             d.part_code,
                             d.part_job_order_no,
                             tag_no = "'" + d.tag_no,
                             d.qty
                         };

            grid.ExportToExcel(output, "Summary_Delivery_Sheet.xls");

            return View();
        }

        //
        // GET: /StampingDaily/
        public ActionResult StampingDaily()
        {
            var tmp = parts.tm_workcenter_stamp.GroupBy(g => g.wc).OrderBy(o => o.Max(m => m.wc_name)).Select(s => new { wc = s.Key, wc_name = s.Max(m => m.wc_name) }).ToList();
            List<List<string>> lst = new List<List<string>>();
            List<string> tmpList;
            foreach (var item in tmp)
            {
                tmpList = new List<string>();
                tmpList.Add(item.wc);
                tmpList.Add(item.wc_name);
                lst.Add(tmpList);
            }
            ViewBag.WC = lst;
            ViewBag.Title = "Stamping Daily Record";

            return View();
        }

        //
        // GET: /GetStampingDailyRecord/

        public ActionResult GetStampingDailyRecord(GridSettings gridSettings, string datefrom, string dateto, string mcno, string user, string wc)
        {
            var data = (from d in parts.td_part_recdata
                        join m in parts.tm_workcenter_stamp on d.wc.Trim() equals m.wc.Trim()
                        select new { 
                            d.wc,
                            d.wc_cure,
                            m.wc_name,
                            d.mc_part,
                            d.part_item,
                            d.finished_goods_code,
                            d.job_order_no,
                            d.tag_no,
                            d.qty_tag,
                            d.insert_date,
                            d.insert_time,
                            d.entry_user
                        }).OrderBy(gridSettings.SortColumn + " " + gridSettings.SortOrder);
            // Filter Condition
            if (!string.IsNullOrEmpty(wc))
            {
                data = data.Where(w => w.wc.Contains(wc));
            }

            if (!string.IsNullOrEmpty(datefrom))
            {
                var dfrom = util.convDateStrToInt(datefrom);
                data = data.Where(w => w.insert_date >= dfrom);
            }

            if (!string.IsNullOrEmpty(dateto))
            {
                var dto = util.convDateStrToInt(dateto);
                data = data.Where(w => w.insert_date <= dto);
            }

            if (!string.IsNullOrEmpty(mcno))
            {
                data = data.Where(w => w.mc_part.Contains(mcno));
            }

            if (!string.IsNullOrEmpty(user))
            {
                data = data.Where(w => w.entry_user.Contains(user));
            }

            // Count All Record
            double count = data.Count();

            // Select Data Per Page
            var dataTmp = from d in data.Skip((gridSettings.PageIndex - 1) * gridSettings.PageSize).Take(gridSettings.PageSize).ToList()
                          select new
                          {
                              d.wc_cure,
                              d.wc_name,
                              d.mc_part,
                              d.part_item,
                              d.finished_goods_code,
                              d.job_order_no,
                              d.tag_no,
                              d.qty_tag,
                              insert_date = util.convDateIntToStr(d.insert_date),
                              insert_time = d.insert_time.ToString(),
                              entry_user = util.convUserNameFormat(d.entry_user)
                          };

            var selectData = dataTmp.Select(d => new
            {
                id = d.job_order_no,
                cell = new object[]
                                        {
                                            d.wc_cure,
                                            d.wc_name,
                                            d.mc_part,
                                            d.part_item,
                                            d.finished_goods_code,
                                            d.job_order_no,
                                            d.tag_no,
                                            d.qty_tag,
                                            d.insert_date,
                                            d.insert_time,
                                            d.entry_user
                                        }
            });

            // Prepare JSON Format
            object jsonData = new
            {
                page = gridSettings.PageIndex,
                total = Math.Ceiling(count / gridSettings.PageSize),
                records = count,
                rows = selectData
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ExportToExcelStampingDailyRecord(string edatefrom, string edateto, string emcno, string euser, string ewc, string sidx, string sord)
        {
            var gridModel = new OrdersJqGridStampingDaily();
            var grid = gridModel.OrdersGrid;

            // Prepare data
            var data = (from d in parts.td_part_recdata
                        join m in parts.tm_workcenter_stamp on d.wc equals m.wc
                        select new
                        {
                            d.wc,
                            d.wc_cure,
                            m.wc_name,
                            d.mc_part,
                            d.part_item,
                            d.finished_goods_code,
                            d.job_order_no,
                            d.tag_no,
                            d.qty_tag,
                            d.insert_date,
                            d.insert_time,
                            d.entry_user
                        }).OrderBy(sidx + " " + sord);
            // Filter Condition
            if (!string.IsNullOrEmpty(ewc))
            {
                data = data.Where(w => w.wc.Contains(ewc));
            }

            if (!string.IsNullOrEmpty(edatefrom))
            {
                var dfrom = util.convDateStrToInt(edatefrom);
                data = data.Where(w => w.insert_date >= dfrom);
            }

            if (!string.IsNullOrEmpty(edateto))
            {
                var dto = util.convDateStrToInt(edateto);
                data = data.Where(w => w.insert_date <= dto);
            }

            if (!string.IsNullOrEmpty(emcno))
            {
                data = data.Where(w => w.mc_part.Contains(emcno));
            }

            if (!string.IsNullOrEmpty(euser))
            {
                data = data.Where(w => w.entry_user.Contains(euser));
            }

            var output = from d in data.ToList()
                         select new
                         {
                             d.wc_cure,
                             d.wc_name,
                             mc_part = "'" + d.mc_part,
                             d.part_item,
                             d.finished_goods_code,
                             d.job_order_no,
                             tag_no = "'" + d.tag_no,
                             d.qty_tag,
                             insert_date = util.convDateIntToStr(d.insert_date),
                             insert_time = "'" + d.insert_time.ToString(),
                             entry_user = util.convUserNameFormat(d.entry_user)
                         };

            grid.ExportToExcel(output, "Stamping_Daily_Report.xls");

            return View();
        }

        //
        // GET: /BondingDaily/
        public ActionResult BondingDaily()
        {
            var tmp = parts.tm_workcenter_bonding.GroupBy(g => g.wc).OrderBy(o => o.Max(m => m.wc_name)).Select(s => new { wc = s.Key, wc_name = s.Max(m => m.wc_name) });
            List<List<string>> lst = new List<List<string>>();
            List<string> tmpList;
            foreach (var item in tmp)
            {
                tmpList = new List<string>();
                tmpList.Add(item.wc);
                tmpList.Add(item.wc_name);
                lst.Add(tmpList);
            }
            ViewBag.WC = lst;
            ViewBag.Title = "Bonding Daily Record";

            return View();
        }

        //
        // GET: /GetBondingDailyRecord/

        public ActionResult GetBondingDailyRecord(GridSettings gridSettings, string datefrom, string dateto, string mcno, string user, string wc, string basketno)
        {
            var data = (from d in parts.td_part_recdata
                        join m in parts.tm_workcenter_bonding on d.wc equals m.wc
                        select new
                        {
                            d.wc,
                            d.wc_cure,
                            m.wc_name,
                            d.mc_part,
                            d.part_item,
                            d.finished_goods_code,
                            d.job_order_no,
                            d.tag_no,
                            d.qty_tag,
                            d.basket_num,
                            d.insert_date,
                            d.insert_time,
                            d.entry_user
                        }).OrderBy(gridSettings.SortColumn + " " + gridSettings.SortOrder);
            // Filter Condition
            if (!string.IsNullOrEmpty(wc))
            {
                data = data.Where(w => w.wc.Contains(wc));
            }

            if (!string.IsNullOrEmpty(datefrom))
            {
                var dfrom = util.convDateStrToInt(datefrom);
                data = data.Where(w => w.insert_date >= dfrom);
            }

            if (!string.IsNullOrEmpty(dateto))
            {
                var dto = util.convDateStrToInt(dateto);
                data = data.Where(w => w.insert_date <= dto);
            }

            if (!string.IsNullOrEmpty(mcno))
            {
                data = data.Where(w => w.mc_part.Contains(mcno));
            }

            if (!string.IsNullOrEmpty(user))
            {
                data = data.Where(w => w.entry_user.Contains(user));
            }

            if (!string.IsNullOrEmpty(basketno))
            {
                data = data.Where(w => w.basket_num.ToString().Contains(basketno));
            }

            // Count All Record
            double count = data.Count();

            // Select Data Per Page
            var dataTmp = from d in data.Skip((gridSettings.PageIndex - 1) * gridSettings.PageSize).Take(gridSettings.PageSize).ToList()
                          select new
                          {
                              d.wc_cure,
                              d.wc_name,
                              d.mc_part,
                              d.part_item,
                              d.finished_goods_code,
                              d.job_order_no,
                              d.tag_no,
                              d.qty_tag,
                              d.basket_num,
                              insert_date = util.convDateIntToStr(d.insert_date),
                              insert_time = d.insert_time.ToString(),
                              entry_user = util.convUserNameFormat(d.entry_user)
                          };

            var selectData = dataTmp.Select(d => new
            {
                id = d.job_order_no,
                cell = new object[]
                                        {
                                            d.wc_cure,
                                            d.wc_name,
                                            d.mc_part,
                                            d.part_item,
                                            d.finished_goods_code,
                                            d.job_order_no,
                                            d.tag_no,
                                            d.qty_tag,
                                            d.basket_num,
                                            d.insert_date,
                                            d.insert_time,
                                            d.entry_user
                                        }
            });

            // Prepare JSON Format
            object jsonData = new
            {
                page = gridSettings.PageIndex,
                total = Math.Ceiling(count / gridSettings.PageSize),
                records = count,
                rows = selectData
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ExportToExcelBondingDailyRecord(string edatefrom, string edateto, string emcno,  string euser, string ewc, string basketno, string sidx, string sord)
        {
            var gridModel = new OrdersJqGridBondingDaily();
            var grid = gridModel.OrdersGrid;

            // Prepare data
            var data = (from d in parts.td_part_recdata
                        join m in parts.tm_workcenter_bonding on d.wc equals m.wc
                        select new
                        {
                            d.wc,
                            d.wc_cure,
                            m.wc_name,
                            d.mc_part,
                            d.part_item,
                            d.finished_goods_code,
                            d.job_order_no,
                            d.tag_no,
                            d.qty_tag,
                            d.basket_num,
                            d.insert_date,
                            d.insert_time,
                            d.entry_user
                        }).OrderBy(sidx + " " + sord);
            // Filter Condition
            if (!string.IsNullOrEmpty(ewc))
            {
                data = data.Where(w => w.wc.Contains(ewc));
            }

            if (!string.IsNullOrEmpty(edatefrom))
            {
                var dfrom = util.convDateStrToInt(edatefrom);
                data = data.Where(w => w.insert_date >= dfrom);
            }

            if (!string.IsNullOrEmpty(edateto))
            {
                var dto = util.convDateStrToInt(edateto);
                data = data.Where(w => w.insert_date <= dto);
            }

            if (!string.IsNullOrEmpty(emcno))
            {
                data = data.Where(w => w.mc_part.Contains(emcno));
            }

            if (!string.IsNullOrEmpty(euser))
            {
                data = data.Where(w => w.entry_user.Contains(euser));
            }

            if (!string.IsNullOrEmpty(basketno))
            {
                data = data.Where(w => w.basket_num.ToString().Contains(basketno));
            }

            var output = from d in data.ToList()
                         select new
                         {
                             d.wc_cure,
                             d.wc_name,
                             mc_part = "'" + d.mc_part,
                             d.part_item,
                             d.finished_goods_code,
                             d.job_order_no,
                             tag_no = "'" + d.tag_no,
                             d.qty_tag,
                             d.basket_num,
                             insert_date = util.convDateIntToStr(d.insert_date),
                             insert_time = "'" + d.insert_time.ToString(),
                             entry_user = util.convUserNameFormat(d.entry_user)
                         };

            grid.ExportToExcel(output, "Bonding_Daily_Report.xls");

            return View();
        }


        public ActionResult SpringDailySupplyJobtag()
        {
            ViewBag.Title = "Spring Daily Supply (Jobtag)";
            return View();
        }

        //
        // GET: /SpringDailySupplyJobtag/

        public ActionResult GetSpringDailySupplyJobtag(GridSettings gridSettings, string plant, string wc, string jobno, string odatefrom, string odateto, string sdatefrom, string sdateto)
        {
            var data = (from d in kanban.td_kanban_info
                        where d.part_type == 2
                        select new
                        {
                            d.location_cd,
                            d.parent_wc,
                            d.parent_mc,
                            d.item,
                            d.job_order_no,
                            d.tag_no,
                            d.qty_tag,
                            d.cure_date,
                            d.order_date,
                            d.order_time,
                            d.order_user,
                            d.supply_date,
                            d.supply_time,
                            d.supply_user
                        }).OrderBy(gridSettings.SortColumn + " " + gridSettings.SortOrder);
            // Filter Condition
            if (!string.IsNullOrEmpty(plant))
            {
                data = data.Where(w => w.location_cd == plant);
            }

            if (!string.IsNullOrEmpty(wc))
            {
                data = data.Where(w => w.parent_wc.Contains(wc));
            }

            if (!string.IsNullOrEmpty(jobno))
            {
                data = data.Where(w => w.job_order_no == jobno);
            }

            if (!string.IsNullOrEmpty(odatefrom))
            {
                var dfrom = util.convDateStrToDBFormatStr(odatefrom);
                data = data.Where(w => w.order_date.CompareTo(dfrom) >= 0);
            }

            if (!string.IsNullOrEmpty(odateto))
            {
                var dto = util.convDateStrToDBFormatStr(odateto);
                data = data.Where(w => w.order_date.CompareTo(dto) <= 0);
            }

            if (!string.IsNullOrEmpty(sdatefrom))
            {
                var dfrom = util.convDateTimeStrToDBFormatStr(sdatefrom);
                data = data.Where(w => (w.supply_date + w.supply_time).CompareTo(dfrom) >= 0);
            }

            if (!string.IsNullOrEmpty(sdateto))
            {
                var dto = util.convDateTimeStrToDBFormatStr(sdateto);
                data = data.Where(w => (w.supply_date + w.supply_time).CompareTo(dto) <= 0);
            }

            // Count All Record
            double count = data.Count();

            // Select Data Per Page
            var dataTmp = from d in data.Skip((gridSettings.PageIndex - 1) * gridSettings.PageSize).Take(gridSettings.PageSize).ToList()
                          select new
                          {
                              d.location_cd,
                              d.parent_wc,
                              d.parent_mc,
                              d.item,
                              d.job_order_no,
                              d.tag_no,
                              d.qty_tag,
                              cure_date = util.convDateIntToStr(d.cure_date),
                              order_date = util.convDateIntToStr(d.order_date),
                              order_user = util.convUserNameFormat(d.order_user),
                              supply_date = util.convDateIntToStr(d.supply_date),
                              supply_time = util.convTimeStrFormat(d.supply_time),
                              supply_user = util.convUserNameFormat(d.supply_user)
                          };

            var selectData = dataTmp.Select(d => new
            {
                id = d.job_order_no,
                cell = new object[]
                                        {
                                            d.location_cd,
                                            d.parent_wc,
                                            d.parent_mc,
                                            d.item,
                                            d.job_order_no,
                                            d.tag_no,
                                            d.qty_tag,
                                            d.cure_date,
                                            d.order_date,
                                            d.order_user,
                                            d.supply_date,
                                            d.supply_time,
                                            d.supply_user
                                        }
            });

            

            // Prepare JSON Format
            object jsonData = new
            {
                page = gridSettings.PageIndex,
                total = Math.Ceiling(count / gridSettings.PageSize),
                records = count,
                rows = selectData
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ExportToExcelSpringDailyJobtag(string plant, string wc, string jobno, string odatefrom, string odateto, string sdatefrom, string sdateto, string sidx, string sord)
        {
            var gridModel = new OrdersJqGridSpringDailyJobtag();
            var grid = gridModel.OrdersGrid;

            // Prepare data
            var data = (from d in kanban.td_kanban_info
                        where d.part_type == 2
                        select new
                        {
                            d.location_cd,
                            d.parent_wc,
                            d.parent_mc,
                            d.item,
                            d.job_order_no,
                            d.tag_no,
                            d.qty_tag,
                            d.cure_date,
                            d.order_date,
                            d.order_time,
                            d.order_user,
                            d.supply_date,
                            d.supply_time,
                            d.supply_user
                        }).OrderBy(sidx + " " + sord);
            // Filter Condition
            if (!string.IsNullOrEmpty(plant))
            {
                data = data.Where(w => w.location_cd == plant);
            }

            if (!string.IsNullOrEmpty(wc))
            {
                data = data.Where(w => w.parent_wc.Contains(wc));
            }

            if (!string.IsNullOrEmpty(jobno))
            {
                data = data.Where(w => w.job_order_no == jobno);
            }

            if (!string.IsNullOrEmpty(odatefrom))
            {
                var dfrom = util.convDateStrToDBFormatStr(odatefrom);
                data = data.Where(w => w.order_date.CompareTo(dfrom) >= 0);
            }

            if (!string.IsNullOrEmpty(odateto))
            {
                var dto = util.convDateStrToDBFormatStr(odateto);
                data = data.Where(w => w.order_date.CompareTo(dto) <= 0);
            }

            if (!string.IsNullOrEmpty(sdatefrom))
            {
                var dfrom = util.convDateTimeStrToDBFormatStr(sdatefrom);
                data = data.Where(w => (w.supply_date + w.supply_time).CompareTo(dfrom) >= 0);
            }

            if (!string.IsNullOrEmpty(sdateto))
            {
                var dto = util.convDateTimeStrToDBFormatStr(sdateto);
                data = data.Where(w => (w.supply_date + w.supply_time).CompareTo(dto) <= 0);
            }

            var output = from d in data.ToList()
                         select new
                         {
                             d.location_cd,
                             d.parent_wc,
                             d.parent_mc,
                             d.item,
                             d.job_order_no,
                             d.tag_no,
                             d.qty_tag,
                             cure_date = util.convDateIntToStr(d.cure_date),
                             order_date = util.convDateIntToStr(d.order_date),
                             order_time = d.order_time.Trim() == string.Empty ? string.Empty : "'" + util.convTimeStrFormat(d.order_time),
                             order_user = util.convUserNameFormat(d.order_user),
                             supply_date = util.convDateIntToStr(d.supply_date),
                             supply_time = d.supply_time.Trim() == string.Empty ? string.Empty : "'" + util.convTimeStrFormat(d.supply_time),
                             supply_user = util.convUserNameFormat(d.supply_user)
                         };

            grid.ExportToExcel(output, "Spring_Daily_Supply_Jobtag_Report.xls");

            return View();
        }

        public ActionResult SpringDailySupplyKanban()
        {
            ViewBag.Title = "Spring Daily Supply (Kanban)";
            return View();
        }

        public ActionResult RubberDailySupplyJobtag()
        {
            ViewBag.Title = "Rubber Daily Supply (Jobtag)";
            return View();
        }

        //
        // GET: /RubberDailySupplyJobtag/

        public ActionResult GetRubberDailySupplyJobtag(GridSettings gridSettings, string plant, string wc, string jobno, string odatefrom, string odateto, string sdatefrom, string sdateto)
        {
            var data = (from d in kanban.td_kanban_info.Where(w => w.part_type == 1)//.ToList()
                        //join a in qim.tr_curing_nics.ToList() on d.parent_job_order_no equals a.job_order_no into g
                        //from a in g.DefaultIfEmpty()
                        select new
                        {
                            d.location_cd,
                            d.parent_wc,
                            d.parent_mc,
                            d.item,
                            d.job_order_no,
                            d.tag_no,
                            d.qty_tag,
                            d.cure_date,
                            d.order_date,
                            d.order_time,
                            d.order_user,
                            d.supply_date,
                            d.supply_time,
                            d.supply_user,
                            d.batch1,
                            d.batch2,
                            d.parent_job_order_no
                            //a.material_mark1
                        }).OrderBy(gridSettings.SortColumn + " " + gridSettings.SortOrder);
            // Filter Condition
            if (!string.IsNullOrEmpty(plant))
            {
                data = data.Where(w => w.location_cd == plant);
            }

            if (!string.IsNullOrEmpty(wc))
            {
                data = data.Where(w => w.parent_wc.Contains(wc));
            }

            if (!string.IsNullOrEmpty(jobno))
            {
                data = data.Where(w => w.job_order_no == jobno);
            }

            if (!string.IsNullOrEmpty(odatefrom))
            {
                var dfrom = util.convDateStrToDBFormatStr(odatefrom);
                data = data.Where(w => w.order_date.CompareTo(dfrom) >= 0);
            }

            if (!string.IsNullOrEmpty(odateto))
            {
                var dto = util.convDateStrToDBFormatStr(odateto);
                data = data.Where(w => w.order_date.CompareTo(dto) <= 0);
            }

            if (!string.IsNullOrEmpty(sdatefrom))
            {
                var dfrom = util.convDateTimeStrToDBFormatStr(sdatefrom);
                data = data.Where(w => (w.supply_date + w.supply_time).CompareTo(dfrom) >= 0);
            }

            if (!string.IsNullOrEmpty(sdateto))
            {
                var dto = util.convDateTimeStrToDBFormatStr(sdateto);
                data = data.Where(w => (w.supply_date + w.supply_time).CompareTo(dto) <= 0);
            }

            // Count All Record
            double count = data.Count();

            // Select Data Per Page
            var dataTmp = from d in data.Skip((gridSettings.PageIndex - 1) * gridSettings.PageSize).Take(gridSettings.PageSize).ToList()
                          select new
                          {
                              d.location_cd,
                              d.parent_wc,
                              d.parent_mc,
                              d.item,
                              d.job_order_no,
                              d.tag_no,
                              d.qty_tag,
                              d.batch1,
                              d.batch2,
                              //d.material_mark1,
                              cure_date = util.convDateIntToStr(d.cure_date),
                              order_date = util.convDateIntToStr(d.order_date),
                              order_user = util.convUserNameFormat(d.order_user),
                              supply_date = util.convDateIntToStr(d.supply_date),
                              supply_time = util.convTimeStrFormat(d.supply_time),
                              supply_user = util.convUserNameFormat(d.supply_user)
                          };

            var selectData = dataTmp.Select(d => new
            {
                id = d.job_order_no,
                cell = new object[]
                        {
                            d.location_cd,
                            d.parent_wc,
                            d.parent_mc,
                            d.item,
                            d.job_order_no,
                            d.tag_no,
                            d.qty_tag,
                            //d.material_mark1,
                            d.batch1,
                            d.batch2,
                            d.cure_date,
                            d.order_date,
                            d.order_user,
                            d.supply_date,
                            d.supply_time,
                            d.supply_user
                        }
            });

            // Prepare JSON Format
            object jsonData = new
            {
                page = gridSettings.PageIndex,
                total = Math.Ceiling(count / gridSettings.PageSize),
                records = count,
                rows = selectData
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ExportToExcelRubberDailyJobtag(string plant, string wc, string jobno, string odatefrom, string odateto, string sdatefrom, string sdateto, string sidx, string sord)
        {
            var gridModel = new OrdersJqGridRubberDailyJobtag();
            var grid = gridModel.OrdersGrid;

            // Prepare data
            var data = (from d in kanban.td_kanban_info
                        where d.part_type == 1
                        select new
                        {
                            d.location_cd,
                            d.parent_wc,
                            d.parent_mc,
                            d.item,
                            d.job_order_no,
                            d.tag_no,
                            d.qty_tag,
                            d.cure_date,
                            d.order_date,
                            d.order_time,
                            d.order_user,
                            d.supply_date,
                            d.supply_time,
                            d.supply_user,
                            d.batch1,
                            d.batch2
                        }).OrderBy(sidx + " " + sord);
            // Filter Condition
            if (!string.IsNullOrEmpty(plant))
            {
                data = data.Where(w => w.location_cd == plant);
            }

            if (!string.IsNullOrEmpty(wc))
            {
                data = data.Where(w => w.parent_wc.Contains(wc));
            }

            if (!string.IsNullOrEmpty(jobno))
            {
                data = data.Where(w => w.job_order_no == jobno);
            }

            if (!string.IsNullOrEmpty(odatefrom))
            {
                var dfrom = util.convDateStrToDBFormatStr(odatefrom);
                data = data.Where(w => w.order_date.CompareTo(dfrom) >= 0);
            }

            if (!string.IsNullOrEmpty(odateto))
            {
                var dto = util.convDateStrToDBFormatStr(odateto);
                data = data.Where(w => w.order_date.CompareTo(dto) <= 0);
            }

            if (!string.IsNullOrEmpty(sdatefrom))
            {
                var dfrom = util.convDateTimeStrToDBFormatStr(sdatefrom);
                data = data.Where(w => (w.supply_date + w.supply_time).CompareTo(dfrom) >= 0);
            }

            if (!string.IsNullOrEmpty(sdateto))
            {
                var dto = util.convDateTimeStrToDBFormatStr(sdateto);
                data = data.Where(w => (w.supply_date + w.supply_time).CompareTo(dto) <= 0);
            }

            var output = from d in data.ToList()
                         select new
                         {
                             d.location_cd,
                             d.parent_wc,
                             d.parent_mc,
                             d.item,
                             d.job_order_no,
                             d.tag_no,
                             d.qty_tag,
                             d.batch1,
                             d.batch2,
                             cure_date = util.convDateIntToStr(d.cure_date),
                             order_date = util.convDateIntToStr(d.order_date),
                             order_time = d.order_time.Trim() == string.Empty ? string.Empty : "'" + util.convTimeStrFormat(d.order_time),
                             order_user = util.convUserNameFormat(d.order_user),
                             supply_date = util.convDateIntToStr(d.supply_date),
                             supply_time = d.supply_time.Trim() == string.Empty ? string.Empty : "'" + util.convTimeStrFormat(d.supply_time),
                             supply_user = util.convUserNameFormat(d.supply_user)
                         };

            grid.ExportToExcel(output, "Rubber_Daily_Supply_Jobtag_Report.xls");

            return View();
        }

        [HttpPost]
        public ActionResult ExportToExcelRubberDailySupplyKanban(string compound, string wc, string jobno, string odatefrom, string odateto, string sdatefrom, string sdateto, string sidx, string sord)
        {
            var gridModel = new OrdersJqGridRubberDailyKanban();
            var grid = gridModel.OrdersGrid;

            // Prepare data
            var data = (from d in rbstock.td_job_information
                        where d.group_cd.Trim() == "DC" || d.group_cd.Trim() == "CBS" || d.group_cd.Trim() == "SNT"
                         || d.group_cd.Trim() == "INJ" || d.group_cd.Trim() == "RBB" || d.group_cd.Trim() == "BUSH"
                        select d).OrderBy(sidx + " " + sord);

            // Filter Condition
            if (!string.IsNullOrEmpty(compound))
            {
                data = data.Where(w => w.compound.Contains(compound));
            }

            if (!string.IsNullOrEmpty(wc))
            {
                data = data.Where(w => w.wc.Contains(wc));
            }

            if (!string.IsNullOrEmpty(jobno))
            {
                data = data.Where(w => w.job_order_no == jobno);
            }

            if (!string.IsNullOrEmpty(odatefrom))
            {
                var dfrom = util.convDateTimeStrToDBFormatStr(odatefrom);
                data = data.Where(w => (w.takeout_date + w.takeout_time).CompareTo(dfrom) >= 0);
            }

            if (!string.IsNullOrEmpty(odateto))
            {
                var dto = util.convDateTimeStrToDBFormatStr(odateto);
                data = data.Where(w => (w.takeout_date + w.takeout_time).CompareTo(dto) <= 0);
            }

            if (!string.IsNullOrEmpty(sdatefrom))
            {
                var dfrom = util.convDateTimeStrToDBFormatStr(sdatefrom);
                data = data.Where(w => (w.supply_date + w.supply_time).CompareTo(dfrom) >= 0);
            }

            if (!string.IsNullOrEmpty(sdateto))
            {
                var dto = util.convDateTimeStrToDBFormatStr(sdateto);
                data = data.Where(w => (w.supply_date + w.supply_time).CompareTo(dto) <= 0);
            }

            var output = from d in data.ToList()
                         select new
                         {
                             Group = d.group_cd,
                             Status = d.job_status.Trim().Equals("1") ? "Order" : (d.job_status.Trim().Equals("2") ? "On Process" : (d.job_status.Trim().Equals("3") ? "Supply" : (d.job_status.Trim().Equals("4") ? "Take Out" : d.job_status))),
                             d.supply_box,
                             d.compound,
                             d.batch,
                             d.batch2,
                             d.batch3,
                             d.batch4,
                             d.batch5,
                             d.batch6,
                             d.batch7,
                             d.batch8,
                             d.kanban_no,
                             d.weight,
                             d.unit,
                             order_date = util.convDateIntToStr(d.order_date),
                             order_time = util.convTimeStrFormat(d.order_time),
                             d.order_user,
                             receive_date = util.convDateIntToStr(d.receive_date),
                             receive_time = util.convTimeStrFormat(d.receive_time),
                             d.receive_user,
                             supply_date = util.convDateIntToStr(d.supply_date),
                             supply_time = util.convTimeStrFormat(d.supply_time),
                             d.supply_user,
                             takeout_date = util.convDateIntToStr(d.takeout_date),
                             takeout_time = util.convTimeStrFormat(d.takeout_time),
                             d.takeout_user,
                             d.wc,
                             d.item,
                             d.job_order_no,
                             tag_no = d.tag_no == null ? "" : (d.tag_no.Trim() == "" ? "" : "'" + d.tag_no)
                         };

            grid.ExportToExcel(output, "Rubber_Daily_Supply_Kanban_Report.xls");

            return View();
        }

        /// ADD function by pin 6/1/16   
        public ActionResult RubberDailySupplyTNCProd()
        {
            ViewBag.Title = "Rubber Daily Supply TNC Production System";
            return View();
        }

        /// ADD function by pin 6/1/16   
        public ActionResult GetRubberDailySupplyTNCProd(GridSettings gridSettings, string plant, string wc, string jobno, string group, string mc, string sdatefrom, string sdateto)
        {
            var data = (from d in tncProd.td_rubber_batch
                        join i in tncProd.td_rubber_info on d.parts_job_order_no equals i.parts_job_order_no into g
                        from i in g.DefaultIfEmpty()
                        select new
                        {
                            location_cd = d.location_cd,
                            wc = d.wc,
                            machine_no = d.machine_no,
                            group_cd = d.group_cd,
                            job_order_no = d.job_order_no,
                            lot_no = d.lot_no,
                            parts_item = d.parts_item,
                            compound = d.compound,
                            batch1 = d.batch1,
                            batch2 = d.batch2,
                            parts_job_order_no = d.parts_job_order_no,
                            i.sent_date,
                            i.sent_time,
                            i.sent_user
                        }).OrderBy(gridSettings.SortColumn + " " + gridSettings.SortOrder);

            // Filter Condition
            if (!string.IsNullOrEmpty(plant))
            {
                data = data.Where(w => w.location_cd == plant);
            }

            if (!string.IsNullOrEmpty(wc))
            {
                data = data.Where(w => w.wc.Contains(wc));
            }

            if (!string.IsNullOrEmpty(jobno))
            {
                data = data.Where(w => w.job_order_no == jobno);
            }

            if (!string.IsNullOrEmpty(group))
            {
                data = data.Where(w => w.group_cd == group);
            }

            if (!string.IsNullOrEmpty(mc))
            {
                data = data.Where(w => w.machine_no == mc);
            }

            if (!string.IsNullOrEmpty(sdatefrom))
            {
                var dfrom = util.convDateTimeStrToDBFormatStr(sdatefrom);
                data = data.Where(w => (w.sent_date + w.sent_time).CompareTo(dfrom) >= 0);
            }

            if (!string.IsNullOrEmpty(sdateto))
            {
                var dto = util.convDateTimeStrToDBFormatStr(sdateto);
                data = data.Where(w => (w.sent_date + w.sent_time).CompareTo(dto) <= 0);
            }

            // Count All Record
            double count = data.Count();

            // Select Data Per Page
            var dataTmp2 = from d in data.Skip((gridSettings.PageIndex - 1) * gridSettings.PageSize).Take(gridSettings.PageSize).ToList()
                           select new
                           {
                               location_id = d.location_cd,
                               wc = d.wc,
                               machine_no = d.machine_no,
                               group_cd = d.group_cd,
                               job_order_no = d.job_order_no,
                               lot_no = d.lot_no,
                               parts_item = d.parts_item,
                               compound = d.compound,
                               batch1 = d.batch1,
                               batch2 = d.batch2,
                               loading_date = d.sent_date,
                               loading_time = d.sent_time,
                               loading_user = d.sent_user
                               //loading_date = GetLoadDate(d.parts_job_order_no),
                               //loading_time = GetLoadTime(d.parts_job_order_no),
                               //loading_user = GetLoadUser(d.parts_job_order_no)
                           };

            var selectData = dataTmp2.Select(dt => new
            {
                id = dt.job_order_no,
                cell = new object[]
                    {
                        dt.location_id,
                        dt.wc,
                        dt.machine_no,
                        dt.group_cd,
                        dt.job_order_no,
                        dt.lot_no,
                        dt.parts_item,
                        dt.compound,
                        dt.batch1,
                        dt.batch2,
                        dt.loading_date,
                        dt.loading_time,
                        dt.loading_user
                    }
            });


            // Prepare JSON Format
            object jsonData = new
            {
                page = gridSettings.PageIndex,
                total = Math.Ceiling(count / gridSettings.PageSize),
                records = count,
                rows = selectData
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        /// ADD function by pin 6/1/16   
        [HttpPost]
        public void ExportToExcelRubberDailyTNCProd(string plant, string wc, string jobno, string group, string mc, string sdatefrom, string sdateto)
        {

            // Prepare data
            var data = (from d in tncProd.td_rubber_batch
                        join i in tncProd.td_rubber_info on d.parts_job_order_no equals i.parts_job_order_no
                        select new
                        {
                            location_cd = d.location_cd,
                            wc = d.wc,
                            machine_no = d.machine_no,
                            group_cd = d.group_cd,
                            job_order_no = d.job_order_no,
                            lot_no = d.lot_no,
                            parts_item = d.parts_item,
                            compound = d.compound,
                            batch1 = d.batch1,
                            batch2 = d.batch2,
                            parts_job_order_no = d.parts_job_order_no,
                            i.sent_date,
                            i.sent_time,
                            i.curing_date,
                            i.order_date,
                            i.order_time,
                            i.due_date,
                            i.due_time,
                            i.completed_date,
                            i.completed_time
                        });

            // Filter Condition
            if (!string.IsNullOrEmpty(plant))
            {
                data = data.Where(w => w.location_cd == plant);
            }

            if (!string.IsNullOrEmpty(wc))
            {
                data = data.Where(w => w.wc.Contains(wc));
            }

            if (!string.IsNullOrEmpty(jobno))
            {
                data = data.Where(w => w.job_order_no == jobno);
            }

            if (!string.IsNullOrEmpty(group))
            {
                data = data.Where(w => w.group_cd == group);
            }

            if (!string.IsNullOrEmpty(mc))
            {
                data = data.Where(w => w.machine_no == mc);
            }

            if (!string.IsNullOrEmpty(sdatefrom))
            {
                var dfrom = util.convDateTimeStrToDBFormatStr(sdatefrom);
                data = data.Where(w => (w.sent_date + w.sent_time).CompareTo(dfrom) >= 0);
            }

            if (!string.IsNullOrEmpty(sdateto))
            {
                var dto = util.convDateTimeStrToDBFormatStr(sdateto);
                data = data.Where(w => (w.sent_date + w.sent_time).CompareTo(dto) <= 0);
            }

            var no = 1;

            var output = data.AsEnumerable().Select(s => new
            {
                No = no++,
                Location = s.location_cd,
                WC = s.wc,
                machine_no = s.machine_no,
                group_cd = s.group_cd,
                job_order_no = s.job_order_no,
                lot_no = s.lot_no,
                parts_item = s.parts_item,
                compound = s.compound,
                batch1 = s.batch1,
                batch2 = s.batch2,
                loading_date = GetLoadDate(s.parts_job_order_no) == "" ? "-" : util.convDateIntToStr(GetLoadDate(s.parts_job_order_no)),
                loading_time = GetLoadTime(s.parts_job_order_no) == "" ? "-" : util.convTimeStrFormat(GetLoadTime(s.parts_job_order_no)),
                loading_user = GetLoadUser(s.parts_job_order_no),
                s.curing_date,
                s.order_date,
                s.order_time,
                s.due_date,
                s.due_time,
                s.completed_date,
                s.completed_time
            });

            ut.CreateExcel(output.ToList());
        }

        /// <summary>
        /// Rubber Daily Supply TPS
        /// </summary>
        /// <returns></returns>
        
        public ActionResult RubberDailySupplyTPS()
        {
            ViewBag.Title = "Rubber Daily Supply TPS";
            return View();
        }

        public ActionResult GetRubberDailySupplyTPS(GridSettings gridSettings, string compound, string jobno, string odatefrom, string odateto, string sdatefrom, string sdateto)
        {
            var data = kanban.td_job_tps_info.Where(w => w.job_type == "JRO").OrderBy(gridSettings.SortColumn + " " + gridSettings.SortOrder);

            // Filter Condition
            if (!string.IsNullOrEmpty(compound))
            {
                data = data.Where(w => w.compound.Contains(compound));
            }

            if (!string.IsNullOrEmpty(jobno))
            {
                data = data.Where(w => w.job_order_no == jobno);
            }

            if (!string.IsNullOrEmpty(odatefrom))
            {
                var dfrom = util.convDateTimeStrToDBFormatStr(odatefrom);
                data = data.Where(w => (w.takeout_date + w.takeout_time).CompareTo(dfrom) >= 0);
            }

            if (!string.IsNullOrEmpty(odateto))
            {
                var dto = util.convDateTimeStrToDBFormatStr(odateto);
                data = data.Where(w => (w.takeout_date + w.takeout_time).CompareTo(dto) <= 0);
            }

            if (!string.IsNullOrEmpty(sdatefrom))
            {
                var dfrom = util.convDateTimeStrToDBFormatStr(sdatefrom);
                data = data.Where(w => (w.supply_date + w.supply_time).CompareTo(dfrom) >= 0);
            }

            if (!string.IsNullOrEmpty(sdateto))
            {
                var dto = util.convDateTimeStrToDBFormatStr(sdateto);
                data = data.Where(w => (w.supply_date + w.supply_time).CompareTo(dto) <= 0);
            }

            // Count All Record
            double count = data.Count();

            // Select Data Per Page
            var selectData = data.Skip((gridSettings.PageIndex - 1) * gridSettings.PageSize).Take(gridSettings.PageSize).ToList().Select(d => new
            {
                id = d.job_order_no,
                cell = new object[]
                {
                    d.rack_no,
                    d.compound,
                    util.convDateIntToStr(d.supply_date),
                    util.convTimeStrFormat(d.supply_time),
                    d.supply_user,
                    util.convDateIntToStr(d.takeout_date),
                    util.convTimeStrFormat(d.takeout_time),
                    d.takeout_user,
                    d.job_order_no,
                    d.qty,
                    d.unit
                }
            });

            // Prepare JSON Format
            object jsonData = new
            {
                page = gridSettings.PageIndex,
                total = Math.Ceiling(count / gridSettings.PageSize),
                records = count,
                rows = selectData
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ExportToExcelRubberDailySupplyTPS(string compound, string jobno, string odatefrom, string odateto, string sdatefrom, string sdateto, string sidx, string sord)
        {
            var gridModel = new OrdersJqGridRubberDailyTPS();
            var grid = gridModel.OrdersGrid;

            // Prepare data
            var data = kanban.td_job_tps_info.Where(w => w.job_type == "JRO").OrderBy(sidx + " " + sord);

            // Filter Condition
            if (!string.IsNullOrEmpty(compound))
            {
                data = data.Where(w => w.compound.Contains(compound));
            }

            if (!string.IsNullOrEmpty(jobno))
            {
                data = data.Where(w => w.job_order_no == jobno);
            }

            if (!string.IsNullOrEmpty(odatefrom))
            {
                var dfrom = util.convDateTimeStrToDBFormatStr(odatefrom);
                data = data.Where(w => (w.takeout_date + w.takeout_time).CompareTo(dfrom) >= 0);
            }

            if (!string.IsNullOrEmpty(odateto))
            {
                var dto = util.convDateTimeStrToDBFormatStr(odateto);
                data = data.Where(w => (w.takeout_date + w.takeout_time).CompareTo(dto) <= 0);
            }

            if (!string.IsNullOrEmpty(sdatefrom))
            {
                var dfrom = util.convDateTimeStrToDBFormatStr(sdatefrom);
                data = data.Where(w => (w.supply_date + w.supply_time).CompareTo(dfrom) >= 0);
            }

            if (!string.IsNullOrEmpty(sdateto))
            {
                var dto = util.convDateTimeStrToDBFormatStr(sdateto);
                data = data.Where(w => (w.supply_date + w.supply_time).CompareTo(dto) <= 0);
            }

            var output = from d in data.ToList()
                         select new
                         {
                            d.rack_no,
                            d.compound,
                            order_date = util.convDateIntToStr(d.order_date),
                            order_time = util.convTimeStrFormat(d.order_time),
                            d.order_user,
                            receive_date = util.convDateIntToStr(d.receive_date),
                            receive_time = util.convTimeStrFormat(d.receive_time),
                            d.receive_user,
                            supply_date = util.convDateIntToStr(d.supply_date),
                            supply_time = util.convTimeStrFormat(d.supply_time),
                            d.supply_user,
                            takeout_date = util.convDateIntToStr(d.takeout_date),
                            takeout_time = util.convTimeStrFormat(d.takeout_time),
                            d.takeout_user,
                            d.batch1,
                            d.batch2,
                            d.batch3,
                            d.batch4,
                            d.batch5,
                            d.batch6,
                            d.batch7,
                            d.batch8,
                            d.job_order_no,
                            d.qty,
                            d.unit
                         };

            grid.ExportToExcel(output, "Rubber_Daily_Supply_TPS_Report.xls");

            return View();
        }

        /// <summary>
        /// Spring Daily Supply TPS
        /// </summary>
        /// <returns></returns>

        public ActionResult SpringDailySupplyTPS()
        {
            ViewBag.Title = "Spring Daily Supply TPS";
            return View();
        }

        public ActionResult GetSpringDailySupplyTPS(GridSettings gridSettings, string jobno, string cdatefrom, string cdateto, string sdatefrom, string sdateto)
        {
            var data = kanban.td_job_tps_info.Where(w => w.job_type == "JSO").OrderBy(gridSettings.SortColumn + " " + gridSettings.SortOrder);

            // Filter Condition
            if (!string.IsNullOrEmpty(jobno))
            {
                data = data.Where(w => w.job_order_no == jobno);
            }

            if (!string.IsNullOrEmpty(cdatefrom))
            {
                var dfrom = util.convDateTimeStrToDBFormatStr(cdatefrom);
                data = data.Where(w => (w.takeout_date + w.takeout_time).CompareTo(dfrom) >= 0);
            }

            if (!string.IsNullOrEmpty(cdateto))
            {
                var dto = util.convDateTimeStrToDBFormatStr(cdateto);
                data = data.Where(w => (w.takeout_date + w.takeout_time).CompareTo(dto) <= 0);
            }

            if (!string.IsNullOrEmpty(sdatefrom))
            {
                var dfrom = util.convDateTimeStrToDBFormatStr(sdatefrom);
                data = data.Where(w => (w.supply_date + w.supply_time).CompareTo(dfrom) >= 0);
            }

            if (!string.IsNullOrEmpty(sdateto))
            {
                var dto = util.convDateTimeStrToDBFormatStr(sdateto);
                data = data.Where(w => (w.supply_date + w.supply_time).CompareTo(dto) <= 0);
            }

            // Count All Record
            double count = data.Count();

            // Select Data Per Page
            var selectData = data.Skip((gridSettings.PageIndex - 1) * gridSettings.PageSize).Take(gridSettings.PageSize).ToList().Select(d => new
            {
                id = d.job_order_no,
                cell = new object[]
                {
                    d.location_cd,
                    d.fg_wc,
                    d.machine_no,
                    d.parent_item,
                    d.job_order_no,
                    d.tag_no,
                    d.qty,
                    util.convDateIntToStr(d.cure_date),
                    d.kanban_no,
                    util.convDateIntToStr(d.supply_date),
                    util.convTimeStrFormat(d.supply_time),
                    d.supply_user
                }
            });

            // Prepare JSON Format
            object jsonData = new
            {
                page = gridSettings.PageIndex,
                total = Math.Ceiling(count / gridSettings.PageSize),
                records = count,
                rows = selectData
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ExportToExcelSpringDailySupplyTPS(string jobno, string cdatefrom, string cdateto, string sdatefrom, string sdateto, string sidx, string sord)
        {
            var gridModel = new OrdersJqGridSpringDailyTPS();
            var grid = gridModel.OrdersGrid;

            // Prepare data
            var data = kanban.td_job_tps_info.Where(w => w.job_type == "JSO").OrderBy(sidx + " " + sord);

            // Filter Condition
            if (!string.IsNullOrEmpty(jobno))
            {
                data = data.Where(w => w.job_order_no == jobno);
            }

            if (!string.IsNullOrEmpty(cdatefrom))
            {
                var dfrom = util.convDateTimeStrToDBFormatStr(cdatefrom);
                data = data.Where(w => (w.cure_date).CompareTo(dfrom) >= 0);
            }

            if (!string.IsNullOrEmpty(cdateto))
            {
                var dto = util.convDateTimeStrToDBFormatStr(cdateto);
                data = data.Where(w => (w.cure_date).CompareTo(dto) <= 0);
            }

            if (!string.IsNullOrEmpty(sdatefrom))
            {
                var dfrom = util.convDateTimeStrToDBFormatStr(sdatefrom);
                data = data.Where(w => (w.supply_date + w.supply_time).CompareTo(dfrom) >= 0);
            }

            if (!string.IsNullOrEmpty(sdateto))
            {
                var dto = util.convDateTimeStrToDBFormatStr(sdateto);
                data = data.Where(w => (w.supply_date + w.supply_time).CompareTo(dto) <= 0);
            }

            var output = from d in data.ToList()
                         select new
                         {
                             d.rack_no,
                             d.compound,
                             order_date = util.convDateIntToStr(d.order_date),
                             order_time = util.convTimeStrFormat(d.order_time),
                             d.order_user,
                             receive_date = util.convDateIntToStr(d.receive_date),
                             receive_time = util.convTimeStrFormat(d.receive_time),
                             d.receive_user,
                             supply_date = util.convDateIntToStr(d.supply_date),
                             supply_time = util.convTimeStrFormat(d.supply_time),
                             d.supply_user,
                             takeout_date = util.convDateIntToStr(d.takeout_date),
                             takeout_time = util.convTimeStrFormat(d.takeout_time),
                             d.takeout_user,
                             d.job_order_no,
                             d.qty,
                             d.unit
                         };

            grid.ExportToExcel(output, "Spring_Daily_Supply_TPS_Report.xls");

            return View();
        }

        //add get loaddate by pin 6/1/16
        public static string GetLoadDate(string part_job_order_no)
        {
            string loadDate = "";
            using (var dbPart = new partnicsEntities())
            {
                var getLoadDate = (from a in dbPart.td_part_delivery
                                   where a.part_job_order_no == part_job_order_no && a.loading_status == "1"
                                   select a).FirstOrDefault();

                if (getLoadDate != null)
                {
                    loadDate = getLoadDate.loading_date.ToString();
                }
            }
            return loadDate;
        }


        //add get loadtime by pin 6/1/16
        public static string GetLoadTime(string part_job_order_no)
        {
            string loadTime = "";
            using (var dbPart = new partnicsEntities())
            {
                var getLoadTime = (from a in dbPart.td_part_delivery
                                   where a.part_job_order_no == part_job_order_no && a.loading_status == "1"
                                   select a).FirstOrDefault();

                if (getLoadTime != null)
                {
                    loadTime = getLoadTime.loading_time.ToString();
                }
            }
            return loadTime;
        }


        //add get loaduser by pin 6/1/16
        public static string GetLoadUser(string part_job_order_no)
        {
            string loadUser = "";
            using (var dbPart = new partnicsEntities())
            {
                var getLoadUser = (from a in dbPart.td_part_delivery
                                   where a.part_job_order_no == part_job_order_no && a.loading_status == "1"
                                   select a).FirstOrDefault();

                if (getLoadUser != null)
                {
                    loadUser = getLoadUser.loading_user.ToString();
                }
            }
            return loadUser;
        }

        public static string GetCompound(string job_order_no)
        {
            string loadComp = "";
            using (var dbQim = new rubberEntities())
            {
                var getLoadComp = (from a in dbQim.tr_rubber_job_nics
                                   where a.job_order_no == job_order_no
                                   select a).FirstOrDefault();

                if (getLoadComp != null)
                {
                    loadComp = getLoadComp.parts_material_code.ToString();
                }
            }
            return loadComp;
        }

        protected override void Dispose(bool disposing)
        {
            parts.Dispose();
            kanban.Dispose();
            rbstock.Dispose();

            base.Dispose(disposing);
        }
    }
}
