using DAO_ReputationService.Contexts;
using DAO_ReputationService.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Helpers.Constants.Enums;
using static DAO_ReputationService.Mapping.AutoMapperBase;
using PagedList.Core;
using Helpers.Models.SharedModels;
using DAO_ReputationService.Mapping;
using Helpers.Constants;
using Helpers.Models.DtoModels.ReputationDbDto;

namespace DAO_ReputationService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserReputationStakeController : ControllerBase
    {

        [Route("Get")]
        [HttpGet]
        public IEnumerable<UserReputationStakeDto> Get()
        {
            List<UserReputationStake> model = new List<UserReputationStake>();

            try
            {
                using (dao_reputationserv_context db = new dao_reputationserv_context())
                {
                    model = db.UserReputationStakes.ToList();
                }
            }
            catch (Exception ex)
            {
                model = new List<UserReputationStake>();
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return _mapper.Map<List<UserReputationStake>, List<UserReputationStakeDto>>(model).ToArray();
        }

        [Route("GetId")]
        [HttpGet]
        public UserReputationStakeDto GetId(int id)
        {
            UserReputationStake model = new UserReputationStake();

            try
            {
                using (dao_reputationserv_context db = new dao_reputationserv_context())
                {
                    model = db.UserReputationStakes.Find(id);
                }
            }
            catch (Exception ex)
            {
                model = new UserReputationStake();
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return _mapper.Map<UserReputationStake, UserReputationStakeDto>(model);
        }

        [Route("Post")]
        [HttpPost]
        public UserReputationStakeDto Post([FromBody] UserReputationStakeDto model)
        {
            try
            {
                UserReputationStake item = _mapper.Map<UserReputationStakeDto, UserReputationStake>(model);
                using (dao_reputationserv_context db = new dao_reputationserv_context())
                {
                    db.UserReputationStakes.Add(item);
                    db.SaveChanges();
                }
                return _mapper.Map<UserReputationStake, UserReputationStakeDto>(item);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return new UserReputationStakeDto();
            }
        }

        [Route("PostMultiple")]
        [HttpPost]
        public List<UserReputationStakeDto> PostMultiple([FromBody] List<UserReputationStakeDto> model)
        {
            try
            {
                List<UserReputationStake> item = _mapper.Map<List<UserReputationStakeDto>, List<UserReputationStake>>(model);
                using (dao_reputationserv_context db = new dao_reputationserv_context())
                {
                    db.UserReputationStakes.AddRange(item);
                    db.SaveChanges();
                }
                return _mapper.Map<List<UserReputationStake>, List<UserReputationStakeDto>>(item);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return new List<UserReputationStakeDto>();
            }
        }

        [Route("Delete")]
        [HttpDelete]
        public bool Delete(int? ID)
        {
            try
            {
                using (dao_reputationserv_context db = new dao_reputationserv_context())
                {
                    UserReputationStake item = db.UserReputationStakes.FirstOrDefault(s => s.UserReputationStakeID == ID);
                    db.Entry(item).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
                    db.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return false;
            }
        }

        [Route("Update")]
        [HttpPut]
        public UserReputationStakeDto Update([FromBody] UserReputationStakeDto model)
        {
            try
            {
                UserReputationStake item = _mapper.Map<UserReputationStakeDto, UserReputationStake>(model);
                using (dao_reputationserv_context db = new dao_reputationserv_context())
                {
                    db.Entry(item).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    db.SaveChanges();
                }
                return _mapper.Map<UserReputationStake, UserReputationStakeDto>(item);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return new UserReputationStakeDto();
            }
        }

        [Route("GetPaged")]
        [HttpGet]
        public PaginationEntity<UserReputationStakeDto> GetPaged(int page = 1, int pageCount = 30)
        {
            PaginationEntity<UserReputationStakeDto> res = new PaginationEntity<UserReputationStakeDto>();

            try
            {
                using (dao_reputationserv_context db = new dao_reputationserv_context())
                {

                    IPagedList<UserReputationStakeDto> lst = AutoMapperBase.ToMappedPagedList<UserReputationStake, UserReputationStakeDto>(db.UserReputationStakes.OrderByDescending(x => x.UserReputationStakeID).ToPagedList(page, pageCount));

                    res.Items = lst;
                    res.MetaData = new PaginationMetaData() { Count = lst.Count, FirstItemOnPage = lst.FirstItemOnPage, HasNextPage = lst.HasNextPage, HasPreviousPage = lst.HasPreviousPage, IsFirstPage = lst.IsFirstPage, IsLastPage = lst.IsLastPage, LastItemOnPage = lst.LastItemOnPage, PageCount = lst.PageCount, PageNumber = lst.PageNumber, PageSize = lst.PageSize, TotalItemCount = lst.TotalItemCount };



                    return res;
                }
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return res;
        }

        [Route("GetByProcessId")]
        [HttpGet]
        public List<UserReputationStakeDto> GetByProcessId(int referenceProcessID, StakeReferenceType reftype)
        {
            List<UserReputationStake> model = new List<UserReputationStake>();

            try
            {
                using (dao_reputationserv_context db = new dao_reputationserv_context())
                {
                    model = db.UserReputationStakes.Where(x=>x.ReferenceProcessID == referenceProcessID && x.ReferenceType == reftype).ToList();
                }
            }
            catch (Exception ex)
            {
                model = new List<UserReputationStake>();
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return _mapper.Map<List<UserReputationStake>, List<UserReputationStakeDto>>(model).ToList();
        }


        [Route("SubmitStake")]
        [HttpPost]
        public SimpleResponse SubmitStake([FromBody] UserReputationStake model)
        {
            SimpleResponse res = new SimpleResponse();

            try
            {
                using (dao_reputationserv_context db = new dao_reputationserv_context())
                {
                    //Check if user already staked reputation for same process
                    if(db.UserReputationStakes.Count(x=>x.ReferenceProcessID == model.ReferenceProcessID && x.UserID == model.UserID) > 0)
                    {
                        return new SimpleResponse() { Success = false, Message = "User have already staked reputation for this process." };
                    }

                    //Check if user have sufficient reputation
                    if (db.UserReputationHistories.Count(x=>x.UserID == model.UserID) > 0 && db.UserReputationHistories.Last(x => x.UserID == model.UserID).LastUsableTotal >= model.Amount)
                    {
                        return new SimpleResponse() { Success = false, Message = "User does not have sufficient reputation." };
                    }

                    //Check if user tries to submit negative stake
                    if (model.Amount <= 0)
                    {
                        return new SimpleResponse() { Success = false, Message = "Reputation stake must be greater than 0" };
                    }

                    model.CreateDate = DateTime.Now;
                    model.Status = ReputationStakeStatus.Staked;

                    db.UserReputationStakes.Add(model);
                    db.SaveChanges();

                    return new SimpleResponse() { Success = true, Message = "Stake successful.", Content = model };
                }
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return res;
        }

        /// <summary>
        ///  This method should be used in cases which staked reputation should be returned to the owner
        /// </summary>
        /// <param name="referenceProcessID"></param>
        /// <returns></returns>
        [Route("ReleaseStakes")]
        [HttpGet]
        public SimpleResponse ReleaseStakes(int referenceProcessID, StakeReferenceType reftype)
        {
            SimpleResponse res = new SimpleResponse();

            try
            {
                using (dao_reputationserv_context db = new dao_reputationserv_context())
                {
                    foreach (var item in db.UserReputationStakes.Where(x=>x.ReferenceProcessID == referenceProcessID && x.ReferenceType == reftype && x.Status == ReputationStakeStatus.Staked))
                    {
                        item.Status = ReputationStakeStatus.Released;
                        db.Entry(item).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                        db.SaveChanges();

                        UserReputationHistory lastReputationHistory = db.UserReputationHistories.Last(x=>x.UserID == item.UserID);

                        UserReputationHistory historyItem = new UserReputationHistory();
                        historyItem.Date = DateTime.Now;
                        historyItem.UserID = item.UserID;
                        historyItem.EarnedAmount = 0;
                        historyItem.LostAmount = 0;
                        historyItem.StakedAmount = 0;
                        historyItem.StakeReleasedAmount = item.Amount;
                        historyItem.LastStakedTotal = lastReputationHistory.LastStakedTotal - item.Amount;
                        historyItem.LastTotal = lastReputationHistory.LastTotal;
                        historyItem.LastUsableTotal = lastReputationHistory.LastUsableTotal + item.Amount;
                        historyItem.Explanation = "Staked reputation released from ProcessId:" + referenceProcessID;

                        db.UserReputationHistories.Add(historyItem);
                        db.SaveChanges();
                    }

                    return new SimpleResponse() { Success = true, Message = "Release successful." };
                }
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return res;
        }

        /// <summary>
        ///  This method should be used in cases which staked reputation should be distributed according to voting results
        /// </summary>
        /// <param name="referenceProcessID"></param>
        /// <returns></returns>
        [Route("DistributeStakes")]
        [HttpGet]
        public SimpleResponse DistributeStakes(int referenceProcessID, StakeReferenceType reftype, Enums.VoteDirection winnerDirection)
        {
            SimpleResponse res = new SimpleResponse();

            try
            {
                using (dao_reputationserv_context db = new dao_reputationserv_context())
                {
                    var stakeList = db.UserReputationStakes.Where(x => x.ReferenceProcessID == referenceProcessID && x.ReferenceType == reftype && x.Status == ReputationStakeStatus.Staked).ToList();
                    var winnersList = stakeList.Where(x => x.Direction == winnerDirection).ToList();
                    var losersList = stakeList.Where(x => x.Direction != winnerDirection).ToList();

                    double losingSideTotalStake = losersList.Sum(x => x.Amount);
                    double winnerSideTotalStake = winnersList.Sum(x => x.Amount);

                    foreach (var item in stakeList)
                    {
                        item.Status = ReputationStakeStatus.Released;
                        db.Entry(item).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                        
                        //User is in the winning side
                        if(winnersList.Count(x=>x.UserID == item.UserID) > 0)
                        { 
                            double usersStakePerc = item.Amount / winnerSideTotalStake;
                            double earnedReputation = losingSideTotalStake * usersStakePerc;

                            UserReputationHistory lastReputationHistory = db.UserReputationHistories.Last(x => x.UserID == item.UserID);

                            UserReputationHistory historyItem = new UserReputationHistory();
                            historyItem.Date = DateTime.Now;
                            historyItem.UserID = item.UserID;
                            historyItem.EarnedAmount = earnedReputation;
                            historyItem.LostAmount = 0;
                            historyItem.StakedAmount = 0;
                            historyItem.StakeReleasedAmount = 0;
                            historyItem.LastStakedTotal = lastReputationHistory.LastStakedTotal;
                            historyItem.LastTotal = lastReputationHistory.LastTotal + earnedReputation;
                            historyItem.LastUsableTotal = lastReputationHistory.LastUsableTotal + earnedReputation;
                            historyItem.Explanation = "User earned repuatation from ProcessId:" + referenceProcessID;
                            db.UserReputationHistories.Add(historyItem);
                        }
                        //User is in the losing side
                        else
                        {
                            UserReputationHistory lastReputationHistory = db.UserReputationHistories.Last(x => x.UserID == item.UserID);

                            UserReputationHistory historyItem = new UserReputationHistory();
                            historyItem.Date = DateTime.Now;
                            historyItem.UserID = item.UserID;
                            historyItem.EarnedAmount = 0;
                            historyItem.LostAmount = item.Amount;
                            historyItem.StakedAmount = 0;
                            historyItem.StakeReleasedAmount = 0;
                            historyItem.LastStakedTotal = lastReputationHistory.LastStakedTotal;
                            historyItem.LastTotal = lastReputationHistory.LastTotal - item.Amount;
                            historyItem.LastUsableTotal = lastReputationHistory.LastUsableTotal - item.Amount;
                            historyItem.Explanation = "User lost repuatation from ProcessId:" + referenceProcessID;
                            db.UserReputationHistories.Add(historyItem);
                        }

                        db.SaveChanges();
                    }

                    return new SimpleResponse() { Success = true, Message = "Distribution successful." };
                }
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return res;
        }
    }
}
