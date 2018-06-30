using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GIVE.Models;

namespace GIVE.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MyWallet(string id)
        {
             givedbEntities ent = new givedbEntities();
            Int32 kidid= Convert.ToInt32(id);
            KidModel model = new KidModel();
            var kidentity = ent.KidBalances.Where(x=>x.Id==kidid).FirstOrDefault();
            if(kidentity!=null)
            {
                model.kidid = kidid;
                model.balance = kidentity.Balance;
                ViewBag.balance = kidentity.Balance.ToString();
                ViewBag.id = id;
                
            }
            return View(model);
        }

        public ActionResult Transaction(string id)
        {
            int kidid= Convert.ToInt32(id);
            givedbEntities ent = new givedbEntities();
            var trans = ent.KidTransactions.Where(x => x.FromKidID == kidid || x.ToAddress == id).ToList();
           
            ViewBag.Transaction = trans;
            return View();
        }


        public ActionResult SendGIV(string id)
        {
            givedbEntities ent = new givedbEntities();
            Int32 kidid = Convert.ToInt32(id);
            ViewBag.id = id;
            ViewBag.kidlist = ent.KidsDatas.Where(x => x.Id != kidid).ToList();
            var receiverval = ent.KidBalances.Where(x => x.KidId == kidid).First();
            ViewBag.balance = receiverval.Balance;
            return View();
        }
        [HttpPost]
        public ActionResult SendGIV(FormCollection frm)
        {
            string parentid = frm["parentid"].ToString();
            string roleValue1 = frm.Get("kiddp");
            string subwallet = frm.Get("subwallet");
            int walletid = 0;
            switch (subwallet)
            {
                case "Play":
                    walletid = 4;
                    break;
                case "Give":
                    walletid = 2;
                    break;
               
            }
            if (roleValue1 == "Select Recipient")
            {
                return View(new { id = parentid });
            }
            else
            {
               
                string amount = frm["givamt"].ToString();
                // string address = frm["givaddress"].ToString();
                int parid = Convert.ToInt32(parentid);
                int amnt = Convert.ToInt32(amount);
                using (givedbEntities ent = new givedbEntities())
                {
                    var fromEntity = ent.KidsDatas.Where(x => x.Id == parid).FirstOrDefault();
                    
                    var ToEntity = ent.KidsDatas.Where(x => x.Name == roleValue1).FirstOrDefault();
                    string comments = amnt.ToString() + " Give Sent To " + ToEntity.Name;
                //    string sentcomments = amnt.ToString() + " Give Received from " + fromEntity.Name;
                    KidTransaction trans = new KidTransaction();
                    trans.FromKidID = parid;
                    trans.ToAddress = ToEntity.Id.ToString();
                    trans.Amount = amnt;
                    trans.Description = comments;
                    //trans.ReceivedDescription = sentcomments;
                    trans.TransactionDate = DateTime.Now;
                    trans.SubWalletType = walletid;
                    ent.KidTransactions.Add(trans);
                    ent.SaveChanges();
                }
                using (givedbEntities ent1 = new givedbEntities())
                {
                    var parent = ent1.KidBalances.Where(x => x.KidId == parid).First();
                    long? newbalance = parent.Balance - amnt;
                    parent.Balance = newbalance;
                    if (walletid == 4)
                    {
                        Int32? playbalance = parent.Play - amnt;
                        parent.Play = playbalance;
                    }
                    if (walletid == 2)
                    {
                        Int32? givbalance = parent.Give - amnt;
                        parent.Give = givbalance;
                    }
                    ent1.SaveChanges();
                }

                using (givedbEntities ent2 = new givedbEntities())
                {
                    var reciever = ent2.KidsDatas.Where(x => x.Name == roleValue1).FirstOrDefault();
                    if (reciever != null)
                    {
                        var receiverval = ent2.KidBalances.Where(x => x.KidId == reciever.Id).First();
                        long? newbalance1 = receiverval.Balance + amnt;
                        receiverval.Balance = newbalance1;
                        if (walletid == 4)
                        {
                            Int32? playbalance = receiverval.Play + amnt;
                            receiverval.Play = playbalance;
                        }
                        if (walletid == 2)
                        {
                            Int32? givbalance = receiverval.Give + amnt;
                            receiverval.Give = givbalance;
                        }
                        ent2.SaveChanges();
                    }
                }

                // ViewBag.id = id;
                return RedirectToAction("MyWallet", "Home", new { id = parentid });
            }
        }
        public ActionResult Intro()
        {
            return View();
        }
        public ActionResult SubWallet(string id)
        {
            Int64 kidid = Convert.ToInt64(id);
            using (givedbEntities ent2 = new givedbEntities())
            {
                var receiverval = ent2.KidBalances.Where(x => x.KidId == kidid).First();
                ViewBag.Freedom = receiverval.Freedom;
                ViewBag.Give = receiverval.Give;
                ViewBag.Play = receiverval.Play;
                ViewBag.Savings = receiverval.Savings;
                ViewBag.Education = receiverval.Education;

            }
            return View();
        }
        [HttpGet]
        public ActionResult checkBalance(string kidid,string amount)
        {
           Int64 amou = Convert.ToInt64(amount);
         
            using (givedbEntities ent2 = new givedbEntities())
            {
                var KIDATA = ent2.KidsDatas.Where(x => x.Name == kidid).First();
                var receiverval = ent2.KidBalances.Where(x => x.KidId == KIDATA.Id).First();
                if ((receiverval.Balance - amou) < 0)
                {
                    return Json("NO", JsonRequestBehavior.AllowGet);
                }
                else {
                    return Json("YES", JsonRequestBehavior.AllowGet);
                }
            }
            
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}