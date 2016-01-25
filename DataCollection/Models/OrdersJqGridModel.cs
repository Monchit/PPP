using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Trirand.Web.Mvc;
using System.Web.UI.WebControls;


namespace DataCollection.Models
{
    public class OrdersJqGridModelNG
    {
        public JQGrid OrdersGrid { get; set; }

        public OrdersJqGridModelNG()
        {
            OrdersGrid = new JQGrid
            {
                Columns = new List<JQGridColumn>()
                    {
                        new JQGridColumn { DataField = "result_date", 
                                        Editable = true,
                                        //DataFormatString = "{0:dd/MM/yyyy}",
                                        Width = 100 },          
                    new JQGridColumn { DataField = "result_time", 
                                        Editable = true,
                                        DataFormatString = "'{0}",
                                        Width = 100 },   
                        new JQGridColumn { DataField = "wc", 
                                        Editable = true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "finished_goods_code",                                                         
                                        Editable = true,
                                        Width = 100 },
                    new JQGridColumn { DataField = "parent_job_order_no", 
                                        // always set PrimaryKey for Add,Edit,Delete operations
                                        // if not set, the first column will be assumed as primary key
                                        PrimaryKey = true,                
                                        Editable = true,
                                        Width = 100 },
                    new JQGridColumn { DataField = "part_code",                                                         
                                        Editable = true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "part_job_order_no", 
                                        Editable = true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "qty",
                                        DataFormatString = "{0:N0}",
                                        Editable =  true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "weight", 
                                        Editable = true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "totalweight", 
                                        Editable = true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "entry_user", 
                                        Editable = true,
                                        Width = 100 }
                    }
            };
        }
    }

    public class OrdersJqGridModelG
    {
        public JQGrid OrdersGrid { get; set; }

        public OrdersJqGridModelG()
        {
            OrdersGrid = new JQGrid
            {
                Columns = new List<JQGridColumn>()
                    {
                        //new JQGridColumn { DataField = "result_date", 
                        //                Editable = true,
                        //                //DataFormatString = "{0:dd/MM/yyyy}",
                        //                Width = 100 },
                        //new JQGridColumn { DataField = "result_time", 
                        //                Editable = true,
                        //                DataFormatString = "'{0}",
                        //                Width = 100 },
                        new JQGridColumn { DataField = "wc", 
                                        Editable = true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "finished_goods_code",                                                         
                                        Editable = true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "parent_job_order_no", 
                                        // always set PrimaryKey for Add,Edit,Delete operations
                                        // if not set, the first column will be assumed as primary key
                                        PrimaryKey = true,                
                                        Editable = true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "qty",
                                        DataFormatString = "{0:N0}",
                                        Editable =  true,
                                        Width = 100 }     ,
                        new JQGridColumn { DataField = "weight", 
                                        Editable = true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "totalweight", 
                                        Editable = true,
                                        Width = 100 }                                
                    }
            };
        }
    }

    public class OrdersJqGridModelSumDelSheet
    {
        public JQGrid OrdersGrid { get; set; }

        public OrdersJqGridModelSumDelSheet()
        {
            OrdersGrid = new JQGrid
            {
                Columns = new List<JQGridColumn>()
                    {
                        
                        new JQGridColumn { DataField = "print_user", 
                                        Editable = true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "print_date", 
                                        Editable = true,
                                        Width = 100 },
                    new JQGridColumn { DataField = "print_time", 
                                        Editable = true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "incoming_date", 
                                        Editable = true,
                                        Width = 100 },
                    new JQGridColumn { DataField = "loading_user", 
                                        Editable = true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "loading_date", 
                                        Editable = true,
                                        Width = 100 },
                    new JQGridColumn { DataField = "loading_time", 
                                        Editable = true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "working_shift",       
                                        Editable = true,
                                        Width = 100 },
                    new JQGridColumn { DataField = "delivery_no",             
                                        Editable = true,
                                        Width = 100 },
                    new JQGridColumn { DataField = "plant",                                                         
                                        Editable = true,
                                        Width = 100 },
                    new JQGridColumn { DataField = "cart_no",                                                         
                                        Editable = true,
                                        Width = 100 },
                    new JQGridColumn { DataField = "truck_no",                                                         
                                        Editable = true,
                                        Width = 100 },
                    new JQGridColumn { DataField = "wc",                                                         
                                        Editable = true,
                                        Width = 100 },
                    new JQGridColumn { DataField = "finished_goods_code",                                                         
                                        Editable = true,
                                        Width = 100 },
                    new JQGridColumn { DataField = "part_code",                                                         
                                        Editable = true,
                                        Width = 100 },
                    new JQGridColumn { DataField = "part_job_order_no", 
                                        PrimaryKey = true,                
                                        Editable = true,
                                        Width = 100 },
                    new JQGridColumn { DataField = "tag_no",                                                         
                                        Editable = true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "qty",
                                        DataFormatString = "{0:N0}",
                                        Editable =  true,
                                        Width = 100 }                                     
                    }
            };
        }
    }

    public class OrdersJqGridModelWaitForReceive
    {
        public JQGrid OrdersGrid { get; set; }

        public OrdersJqGridModelWaitForReceive()
        {
            OrdersGrid = new JQGrid
            {
                Columns = new List<JQGridColumn>()
                    {
                        new JQGridColumn { DataField = "loading_user", 
                                        Editable = true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "loading_date", 
                                        Editable = true,
                                        Width = 100 },
                    new JQGridColumn { DataField = "loading_time", 
                                        Editable = true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "working_shift",       
                                        Editable = true,
                                        Width = 100 },
                    new JQGridColumn { DataField = "delivery_no",             
                                        Editable = true,
                                        Width = 100 },
                    new JQGridColumn { DataField = "plant",                                                         
                                        Editable = true,
                                        Width = 100 },
                    new JQGridColumn { DataField = "cart_no",                                                         
                                        Editable = true,
                                        Width = 100 },
                    new JQGridColumn { DataField = "truck_no",                                                         
                                        Editable = true,
                                        Width = 100 },
                    new JQGridColumn { DataField = "wc",                                                         
                                        Editable = true,
                                        Width = 100 },
                    new JQGridColumn { DataField = "finished_goods_code",                                                         
                                        Editable = true,
                                        Width = 100 },
                    new JQGridColumn { DataField = "part_code",                                                         
                                        Editable = true,
                                        Width = 100 },
                    new JQGridColumn { DataField = "part_job_order_no", 
                                        PrimaryKey = true,                
                                        Editable = true,
                                        Width = 100 },
                    new JQGridColumn { DataField = "tag_no",                                                         
                                        Editable = true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "qty",
                                        DataFormatString = "{0:N0}",
                                        Editable =  true,
                                        Width = 100 }                                     
                    }
            };
        }
    }

    public class OrdersJqGridModelResultByMachine
    {
        public JQGrid OrdersGrid { get; set; }

        public OrdersJqGridModelResultByMachine()
        {
            OrdersGrid = new JQGrid
            {
                Columns = new List<JQGridColumn>()
                {
                new JQGridColumn { DataField = "result_date", 
                                    Editable = true,
                                    Width = 100 },
                new JQGridColumn { DataField = "result_time", 
                                    Editable = true,
                                    Width = 100 },
                new JQGridColumn { DataField = "wc", 
                                    Editable = true,
                                    Width = 100 },
                new JQGridColumn { DataField = "finished_goods_code",                
                                    Editable = true,
                                    Width = 100 },
                new JQGridColumn { DataField = "parent_job_order_no", 
                                    Editable = true,
                                    Width = 100 },
                new JQGridColumn { DataField = "part_code",            
                                    Editable = true,
                                    Width = 100 },
                new JQGridColumn { DataField = "part_job_order_no", 
                                    PrimaryKey = true,           
                                    Editable = true,
                                    Width = 100 },
                new JQGridColumn { DataField = "qty",
                                    DataFormatString = "{0:N0}",
                                    Editable =  true,
                                    Width = 100 },
                new JQGridColumn { DataField = "entry_user",         
                                    Editable = true,
                                    Width = 100 }
                }
            };
        }
    }

    public class OrdersJqGridStampingDaily
    {
        public JQGrid OrdersGrid { get; set; }

        public OrdersJqGridStampingDaily()
        {
            OrdersGrid = new JQGrid
            {
                Columns = new List<JQGridColumn>()
                    {
                        new JQGridColumn { DataField = "wc_cure", 
                                        Editable = true,
                                        Width = 100 },          
                    new JQGridColumn { DataField = "wc_name", 
                                        Editable = true,
                                        Width = 100 },   
                        new JQGridColumn { DataField = "mc_part", 
                                        Editable = true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "part_item",
                                        Editable = true,
                                        Width = 100 },
                    new JQGridColumn { DataField = "finished_goods_code",
                                        Width = 100 },
                    new JQGridColumn { DataField = "job_order_no",                                                                                              PrimaryKey = true,    
                                        Editable = true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "tag_no",
                                        Editable = true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "qty_tag",
                                        DataFormatString = "{0:N0}",
                                        Editable =  true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "insert_date",
                                        DataFormatString = "{0:dd/MM/yyyy}", 
                                        Editable =  true,
                                        Width = 100},
                        new JQGridColumn { DataField = "insert_time",
                                        DataFormatString = "{0:hh:mm:ss}",
                                        Editable =  true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "entry_user",
                                        Editable =  true,
                                        Width = 100 }
                    }
            };
        }
    }

    public class OrdersJqGridBondingDaily
    {
        public JQGrid OrdersGrid { get; set; }

        public OrdersJqGridBondingDaily()
        {
            OrdersGrid = new JQGrid
            {
                Columns = new List<JQGridColumn>()
                    {
                        new JQGridColumn { DataField = "wc_cure", 
                                        Editable = true,
                                        Width = 100 },          
                    new JQGridColumn { DataField = "wc_name", 
                                        Editable = true,
                                        Width = 100 },   
                        new JQGridColumn { DataField = "mc_part", 
                                        Editable = true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "part_item",
                                        Editable = true,
                                        Width = 100 },
                    new JQGridColumn { DataField = "finished_goods_code",
                                        Width = 100 },
                    new JQGridColumn { DataField = "job_order_no",                                                                                              PrimaryKey = true,    
                                        Editable = true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "tag_no",
                                        Editable = true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "qty_tag",
                                        DataFormatString = "{0:N0}",
                                        Editable =  true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "basket_num",
                                        Editable = true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "insert_date",
                                        DataFormatString = "{0:dd/MM/yyyy}", 
                                        Editable =  true,
                                        Width = 100},
                        new JQGridColumn { DataField = "insert_time",
                                        DataFormatString = "{0:hh:mm:ss}",
                                        Editable =  true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "entry_user",
                                        Editable =  true,
                                        Width = 100 }
                    }
            };
        }
    }

    public class OrdersJqGridRubberDailyJobtag
    {
        public JQGrid OrdersGrid { get; set; }

        public OrdersJqGridRubberDailyJobtag()
        {
            OrdersGrid = new JQGrid
            {
                Columns = new List<JQGridColumn>()
                    {
                        new JQGridColumn { DataField = "location_cd", 
                                        Editable = true,
                                        Width = 100 },          
                    new JQGridColumn { DataField = "parent_wc", 
                                        Editable = true,
                                        Width = 100 },   
                        new JQGridColumn { DataField = "parent_mc", 
                                        Editable = true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "item",
                                        Editable = true,
                                        Width = 100 },
                    new JQGridColumn { DataField = "job_order_no", PrimaryKey = true,
                                        Width = 100 },
                    new JQGridColumn { DataField = "tag_no",
                                        DataFormatString = "'{0}",
                                        Editable = true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "qty_tag",
                                        DataFormatString = "{0:N}",
                                        Editable =  true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "cure_date",
                                        DataFormatString = "{0:dd/MM/yyyy}", 
                                        Editable =  true,
                                        Width = 100},
                        new JQGridColumn { DataField = "order_date",
                                        DataFormatString = "{0:dd/MM/yyyy}", 
                                        Editable =  true,
                                        Width = 100},
                        new JQGridColumn { DataField = "order_time",
                                        Editable =  true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "order_user",
                                        Editable =  true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "supply_date",
                                        DataFormatString = "{0:dd/MM/yyyy}", 
                                        Editable =  true,
                                        Width = 100},
                        new JQGridColumn { DataField = "supply_time",
                                        Editable =  true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "supply_user",
                                        Editable =  true,
                                        Width = 100 }
                    }
            };
        }
    }

    public class OrdersJqGridSpringDailyJobtag
    {
        public JQGrid OrdersGrid { get; set; }

        public OrdersJqGridSpringDailyJobtag()
        {
            OrdersGrid = new JQGrid
            {
                Columns = new List<JQGridColumn>()
                    {
                        new JQGridColumn { DataField = "location_cd", 
                                        Editable = true,
                                        Width = 100 },          
                    new JQGridColumn { DataField = "parent_wc", 
                                        Editable = true,
                                        Width = 100 },   
                        new JQGridColumn { DataField = "parent_mc", 
                                        Editable = true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "item",
                                        Editable = true,
                                        Width = 100 },
                    new JQGridColumn { DataField = "job_order_no", PrimaryKey = true,
                                        Width = 100 },
                    new JQGridColumn { DataField = "tag_no",
                                        DataFormatString = "'{0}",
                                        Editable = true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "qty_tag",
                                        DataFormatString = "{0:N0}",
                                        Editable =  true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "cure_date",
                                        DataFormatString = "{0:dd/MM/yyyy}", 
                                        Editable =  true,
                                        Width = 100},
                        new JQGridColumn { DataField = "order_date",
                                        DataFormatString = "{0:dd/MM/yyyy}", 
                                        Editable =  true,
                                        Width = 100},
                        new JQGridColumn { DataField = "order_time",
                                        Editable =  true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "order_user",
                                        Editable =  true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "supply_date",
                                        DataFormatString = "{0:dd/MM/yyyy}", 
                                        Editable =  true,
                                        Width = 100},
                        new JQGridColumn { DataField = "supply_time",
                                        Editable =  true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "supply_user",
                                        Editable =  true,
                                        Width = 100 }
                    }
            };
        }
    }

    public class OrdersJqGridRubberDailyKanban
    {
        public JQGrid OrdersGrid { get; set; }

        public OrdersJqGridRubberDailyKanban()
        {
            OrdersGrid = new JQGrid
            {
                Columns = new List<JQGridColumn>()
                    {
                        new JQGridColumn { DataField = "Group", 
                                        Editable = true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "Status", 
                                        Editable = true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "supply_box", 
                                        Editable = true,
                                        Width = 100 },          
                    new JQGridColumn { DataField = "compound", 
                                        Editable = true,
                                        Width = 100 },   
                        new JQGridColumn { DataField = "batch",
                                        Editable = true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "batch2",
                                        Editable = true,
                                        Width = 100 },
                    new JQGridColumn { DataField = "batch3",
                                        Editable = true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "batch4",
                                        Editable = true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "batch5",
                                        Editable = true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "batch6",
                                        Editable = true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "batch7",
                                        Editable = true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "batch8",
                                        Editable = true,
                                        Width = 100 },
                    new JQGridColumn { DataField = "kanban_no",
                                        DataFormatString = "{0}",
                                        Editable = true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "weight",
                                        DataFormatString = "{0:N2}",
                                        Editable =  true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "unit",
                                        Editable = true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "order_date", DataFormatString = "{0}",
                                        Editable =  true,
                                        Width = 100},
                        new JQGridColumn { DataField = "order_time", DataFormatString = "'{0}",
                                        Editable =  true,
                                        Width = 100},
                        new JQGridColumn { DataField = "order_user",
                                        Editable =  true,
                                        Width = 100},
                        new JQGridColumn { DataField = "receive_date", DataFormatString = "{0}",
                                        Editable =  true,
                                        Width = 100},
                        new JQGridColumn { DataField = "receive_time", DataFormatString = "'{0}",
                                        Editable =  true,
                                        Width = 100},
                        new JQGridColumn { DataField = "receive_user",
                                        Editable =  true,
                                        Width = 100},
                        new JQGridColumn { DataField = "supply_date", DataFormatString = "{0}",
                                        Editable =  true,
                                        Width = 100},
                        new JQGridColumn { DataField = "supply_time", DataFormatString = "'{0}",
                                        Editable =  true,
                                        Width = 100},
                        new JQGridColumn { DataField = "supply_user",
                                        Editable =  true,
                                        Width = 100},
                        new JQGridColumn { DataField = "takeout_date", DataFormatString = "{0}",
                                        Editable =  true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "takeout_time", DataFormatString = "'{0}",
                                        Editable =  true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "takeout_user",
                                        Editable =  true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "wc",
                                        Editable =  true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "item",
                                        Editable =  true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "job_order_no", PrimaryKey = true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "tag_no",
                                        Editable =  true,
                                        Width = 100 }
                    }
            };
        }
    }

    public class OrdersJqGridRubberDailyTPS
    {
        public JQGrid OrdersGrid { get; set; }

        public OrdersJqGridRubberDailyTPS()
        {
            OrdersGrid = new JQGrid
            {
                Columns = new List<JQGridColumn>()
                    {
                        new JQGridColumn { DataField = "rack_no", 
                                        Editable = true,
                                        Width = 100 },          
                    new JQGridColumn { DataField = "compound", 
                                        Editable = true,
                                        Width = 100 },   
                        new JQGridColumn { DataField = "order_date",
                                        Editable = true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "order_time",
                                        Editable = true,
                                        Width = 100 },
                    new JQGridColumn { DataField = "order_user",
                                        Editable = true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "receive_date",
                                        Editable = true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "receive_time",
                                        Editable = true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "receive_user",
                                        Editable = true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "supply_date",
                                        Editable = true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "supply_time",
                                        Editable = true,
                                        Width = 100 },
                    new JQGridColumn { DataField = "supply_user",
                                        Editable = true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "takeout_date",
                                        Editable =  true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "takeout_time",
                                        Editable =  true,
                                        Width = 100},
                        new JQGridColumn { DataField = "takeout_user",
                                        Editable =  true,
                                        Width = 100},
                        new JQGridColumn { DataField = "batch1",
                                        Editable =  true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "batch2",
                                        Editable =  true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "batch3", 
                                        Editable =  true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "batch4", 
                                        Editable =  true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "batch5", 
                                        Editable =  true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "batch6", 
                                        Editable =  true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "batch7", 
                                        Editable =  true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "batch8", 
                                        Editable =  true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "job_order_no", PrimaryKey = true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "qty",
                                        DataFormatString = "{0:N2}",
                                        Editable =  true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "unit",
                                        Editable =  true,
                                        Width = 100 }
                    }
            };
        }
    }

    public class OrdersJqGridSpringDailyTPS
    {
        public JQGrid OrdersGrid { get; set; }

        public OrdersJqGridSpringDailyTPS()
        {
            OrdersGrid = new JQGrid
            {
                Columns = new List<JQGridColumn>()
                    {
                        new JQGridColumn { DataField = "rack_no", 
                                        Editable = true,
                                        Width = 100 },          
                    new JQGridColumn { DataField = "compound", 
                                        Editable = true,
                                        Width = 100 },   
                        new JQGridColumn { DataField = "order_date",
                                        Editable = true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "order_time",
                                        Editable = true,
                                        Width = 100 },
                    new JQGridColumn { DataField = "order_user",
                                        Editable = true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "receive_date",
                                        Editable = true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "receive_time",
                                        Editable = true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "receive_user",
                                        Editable = true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "supply_date",
                                        Editable = true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "supply_time",
                                        Editable = true,
                                        Width = 100 },
                    new JQGridColumn { DataField = "supply_user",
                                        Editable = true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "takeout_date",
                                        Editable =  true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "takeout_time",
                                        Editable =  true,
                                        Width = 100},
                        new JQGridColumn { DataField = "takeout_user",
                                        Editable =  true,
                                        Width = 100},
                        new JQGridColumn { DataField = "job_order_no", PrimaryKey = true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "qty",
                                        DataFormatString = "{0:N2}",
                                        Editable =  true,
                                        Width = 100 },
                        new JQGridColumn { DataField = "unit",
                                        Editable =  true,
                                        Width = 100 }
                    }
            };
        }
    }

}