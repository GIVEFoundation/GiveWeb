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
            Int32 kidid = Convert.ToInt32(id);
            KidModel model = new KidModel();
            var kidentity = ent.KidBalances.Where(x => x.Id == kidid).FirstOrDefault();
            if (kidentity != null)
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
            int kidid = Convert.ToInt32(id);
            givedbEntities ent = new givedbEntities();
            var trans = ent.KidTransactions.Where(x => x.FromKidID == kidid || x.ToAddress == id).ToList();

            ViewBag.Transaction = trans;
            ViewBag.id = kidid;
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
        public ActionResult CharityDonation(FormCollection frm)
        {
            string charityid = frm["charirtyid"].ToString();
            string userid = frm["fromid"].ToString();
            string amount = frm["givamt"].ToString();
            int amt= Convert.ToInt32(amount);
            int charitygrpid = Convert.ToInt32(charityid);
            Int64 user = Convert.ToInt64(userid);
            using (givedbEntities ent = new givedbEntities())
            {
                var fromEntity = ent.KidsDatas.Where(x => x.Id == user).FirstOrDefault();
                var frombalance = ent.KidBalances.Where(x => x.KidId == fromEntity.Id).First();
               
                
                var newfromid = frombalance.Give - amt;
                frombalance.Give = newfromid;
                frombalance.Balance = frombalance.Balance - amt;
                ent.SaveChanges();
            }
            using (givedbEntities ent1 = new givedbEntities())
            {

                var grp = ent1.CharityGroups.Where(x => x.Id == charitygrpid).First().KidsData;

                var balance = ent1.KidBalances.Where(x => x.KidId == grp.Id).First();
                var newfromid = balance.Give + amt;
                balance.Give = newfromid;
                balance.Balance = balance.Balance + amt;
                ent1.SaveChanges();
            }
            using (givedbEntities entnew = new givedbEntities())
            {
                var fromEntity = entnew.KidsDatas.Where(x => x.Id == user).FirstOrDefault();

                var grp = entnew.CharityGroups.Where(x => x.Id == charitygrpid).First().KidsData;
                string comments = amt.ToString() + " Give Sent To " + grp.Name;
                //    string sentcomments = amnt.ToString() + " Give Received from " + fromEntity.Name;
                KidTransaction trans = new KidTransaction();
                trans.FromKidID = fromEntity.Id;
                trans.ToAddress = grp.Id.ToString();
                trans.Amount = amt;
                trans.Description = comments;
                //trans.ReceivedDescription = sentcomments;
                trans.TransactionDate = DateTime.Now;
                trans.SubWalletType = 2;
                entnew.KidTransactions.Add(trans);
                entnew.SaveChanges();
            }
            return RedirectToAction("MyWallet", "Home", new { id = userid });
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
        public ActionResult CharityGroup()
        {
            return View();
        }
        public ActionResult CharityDonation(string id, string userid)
        {
            Int64 kidid = Convert.ToInt64(userid);
            int charityid = Convert.ToInt32(id);
            ViewBag.charityid = id;
            ViewBag.fromid = userid;
            using (givedbEntities ent2 = new givedbEntities())
        {
                var receiverval = ent2.KidBalances.Where(x => x.Id == kidid).First();
                ViewBag.givabalance = receiverval.Give;

                var charitygroup = ent2.CharityGroups.Where(x => x.Id == charityid).First();
                ViewBag.charityname = charitygroup.CharityName;
            }
            return View();
        }
        public ActionResult Charity(string id,string userid)
        {
             int charityId = Convert.ToInt32(id);
             using (givedbEntities ent2 = new givedbEntities())
             {
                 var receiverval = ent2.CharityGroups.Where(x => x.Id == charityId).First();

                 ViewBag.image = receiverval.ImageLink;
                 ViewBag.desc = receiverval.Description;
                 ViewBag.Name = receiverval.CharityName;
                 ViewBag.id = id;
                 ViewBag.userid = userid;
             }
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
            ViewBag.id = id;
            return View();
        }
        [HttpGet]
        public ActionResult checkBalance(string kidid, string amount)
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
                else
                {
                    return Json("YES", JsonRequestBehavior.AllowGet);
                }
            }

        }



        [HttpGet]
        public ActionResult SubWalletTransferConfirm(int kidid, string sourceSubWallet, string targetSubWallet, int amt)
        {
            int subWalletTypeId = 1;
            switch (sourceSubWallet)
            {
                case "freedom":
                    subWalletTypeId = 1;
                    break;
                case "saving":
                    subWalletTypeId = 5;
                    break;
                case "give":
                    subWalletTypeId = 2;
                    break;
                case "play":
                    subWalletTypeId = 4;
                    break;
                case "education":
                    subWalletTypeId = 3;
                    break;
                default:
                    subWalletTypeId = 1;
                    break;
            }

            using (givedbEntities ent = new givedbEntities())
            {
                KidTransaction trans = new KidTransaction();
                trans.FromKidID = kidid;
                trans.ToAddress = kidid.ToString();
                trans.Amount = amt;
                trans.Description = "transferred from '" + sourceSubWallet + "' sub-wallet to '" + targetSubWallet + "' sub-wallet.";
                trans.TransactionDate = DateTime.Now;
                trans.SubWalletType = subWalletTypeId;
                ent.KidTransactions.Add(trans);
                ent.SaveChanges();
            }
            using (givedbEntities ent1 = new givedbEntities())
            {
                var parent = ent1.KidBalances.Where(x => x.KidId == kidid).First();

                switch (sourceSubWallet)
                {
                    case "freedom":
                        parent.Freedom = (parent.Freedom - amt);
                        break;
                    case "saving":
                        parent.Savings = (parent.Savings - amt);
                        break;
                    case "give":
                        parent.Give = (parent.Give - amt);
                        break;
                    case "play":
                        parent.Play = (parent.Play - amt);
                        break;
                    case "education":
                        parent.Education = (parent.Education - amt);
                        break;
                    default:
                        break;
                }
                ent1.SaveChanges();
            }

            using (givedbEntities ent2 = new givedbEntities())
            {
                var reciever = ent2.KidBalances.Where(x => x.Id == kidid).FirstOrDefault();

                switch (targetSubWallet)
                {
                    case "freedom":
                        reciever.Freedom = (reciever.Freedom + amt);
                        break;
                    case "saving":
                        reciever.Savings = (reciever.Savings + amt);
                        break;
                    case "give":
                        reciever.Give = (reciever.Give + amt);
                        break;
                    case "play":
                        reciever.Play = (reciever.Play + amt);
                        break;
                    case "education":
                        reciever.Education = (reciever.Education + amt);
                        break;
                    default:
                        break;
                }
                ent2.SaveChanges();
            }

            return Json(true, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public ActionResult GetBalance(int kidid, string walletType)
        {
            using (givedbEntities ent2 = new givedbEntities())
            {
                int? bal = 0;
                var walletBal = ent2.KidBalances.Where(x => x.KidId == kidid).First();

                switch (walletType)
                {
                    case "freedom":
                        bal = walletBal.Freedom;
                        break;
                    case "saving":
                        bal = walletBal.Savings;
                        break;
                    case "give":
                        bal = walletBal.Give;
                        break;
                    case "play":
                        bal = walletBal.Play;
                        break;
                    case "education":
                        bal = walletBal.Education;
                        break;
                    case "balance":
                        bal = Convert.ToInt32((walletBal.Balance ?? 0));
                        break;
                    default:
                        bal = 0;
                        break;
                }

                return Json((bal ?? 0).ToString(), JsonRequestBehavior.AllowGet);
            }

            }

        public ActionResult SubWalletTransfer(string id, string subWallet)
        {
            ViewBag.id = id;
            ViewBag.selectedSourceSubWallet = subWallet;
            return View();
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