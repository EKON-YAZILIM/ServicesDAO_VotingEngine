using DAO_VotingEngine.Contexts;
using DAO_VotingEngine.Models;
using Helpers.Models.DtoModels.VoteDbDto;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Helpers.Constants.Enums;
using static DAO_VotingEngine.Mapping.AutoMapperBase;
using PagedList.Core;
using DAO_VotingEngine.Mapping;
using Helpers.Models.SharedModels;
using Helpers.Constants;

namespace DAO_VotingEngine.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class VotingController : Controller
    {
        [Route("Get")]
        [HttpGet]
        public IEnumerable<VotingDto> Get()
        {
            List<Voting> model = new List<Voting>();

            try
            {
                using (dao_votesdb_context db = new dao_votesdb_context())
                {
                    model = db.Votings.ToList();
                }
            }
            catch (Exception ex)
            {
                model = new List<Voting>();
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return _mapper.Map<List<Voting>, List<VotingDto>>(model).ToArray();
        }

        [Route("GetId")]
        [HttpGet]
        public VotingDto GetId(int id)
        {
            Voting model = new Voting();

            try
            {
                using (dao_votesdb_context db = new dao_votesdb_context())
                {
                    model = db.Votings.Find(id);
                }
            }
            catch (Exception ex)
            {
                model = new Voting();
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return _mapper.Map<Voting, VotingDto>(model);
        }

        [Route("Post")]
        [HttpPost]
        public VotingDto Post([FromBody] VotingDto model)
        {
            try
            {
                Voting item = _mapper.Map<VotingDto, Voting>(model);
                using (dao_votesdb_context db = new dao_votesdb_context())
                {
                    db.Votings.Add(item);
                    db.SaveChanges();
                }
                return _mapper.Map<Voting, VotingDto>(item);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return new VotingDto();
            }
        }

        [Route("PostMultiple")]
        [HttpPost]
        public List<VotingDto> PostMultiple([FromBody] List<VotingDto> model)
        {
            try
            {
                List<Voting> item = _mapper.Map<List<VotingDto>, List<Voting>>(model);
                using (dao_votesdb_context db = new dao_votesdb_context())
                {
                    db.Votings.AddRange(item);
                    db.SaveChanges();
                }
                return _mapper.Map<List<Voting>, List<VotingDto>>(item);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return new List<VotingDto>();
            }
        }

        [Route("Delete")]
        [HttpDelete]
        public bool Delete(int? ID)
        {
            try
            {
                using (dao_votesdb_context db = new dao_votesdb_context())
                {
                    Voting item = db.Votings.FirstOrDefault(s => s.VotingID == ID);
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
        public VotingDto Update([FromBody] VotingDto model)
        {
            try
            {
                Voting item = _mapper.Map<VotingDto, Voting>(model);
                using (dao_votesdb_context db = new dao_votesdb_context())
                {
                    db.Entry(item).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    db.SaveChanges();
                }
                return _mapper.Map<Voting, VotingDto>(item);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return new VotingDto();
            }
        }

        [Route("GetPaged")]
        [HttpGet]
        public PaginationEntity<VotingDto> GetPaged(int page = 1, int pageCount = 30)
        {
            PaginationEntity<VotingDto> res = new PaginationEntity<VotingDto>();

            try
            {
                using (dao_votesdb_context db = new dao_votesdb_context())
                {

                    IPagedList<VotingDto> lst = AutoMapperBase.ToMappedPagedList<Voting, VotingDto>(db.Votings.OrderByDescending(x => x.VotingID).ToPagedList(page, pageCount));

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

        [Route("GetVotingByStatus")]
        [HttpGet]
        public List<VotingDto> GetVotingByStatus(Enums.VoteStatusTypes? status)
        {
            List<Voting> model = new List<Voting>();

            try
            {
                using (dao_votesdb_context db = new dao_votesdb_context())
                {
                    if (status != null)
                    {
                        model = db.Votings.Where(x => x.Status == status).ToList();
                    }
                    else
                    {
                        model = db.Votings.ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                model = new List<Voting>();
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return _mapper.Map<List<Voting>, List<VotingDto>>(model).ToList();
        }

        [Route("GetByJobId")]
        [HttpGet]
        public VotingDto GetByJobId(int jobid)
        {
            VotingDto res = new VotingDto();

            try
            {
                using (dao_votesdb_context db = new dao_votesdb_context())
                {
                    Voting vt = db.Votings.OrderByDescending(x => x.VotingID).FirstOrDefault(x => x.JobID == jobid);

                    res = _mapper.Map<Voting, VotingDto>(vt);
                }
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return res;
        }

        [Route("GetInformalVotingByJobId")]
        [HttpGet]
        public VotingDto GetInformalVotingByJobId(int jobid)
        {
            VotingDto res = new VotingDto();

            try
            {
                using (dao_votesdb_context db = new dao_votesdb_context())
                {
                    Voting vt = db.Votings.Where(x=>x.IsFormal == false && x.JobID == jobid).OrderByDescending(x => x.VotingID).FirstOrDefault();

                    res = _mapper.Map<Voting, VotingDto>(vt);
                }
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return res;
        }

        [Route("GetCompletedVotingsByJobIds")]
        [HttpPost]
        public List<VotingDto> GetCompletedVotingsByJobIds(List<int> jobids)
        {
            List<VotingDto> res = new List<VotingDto>();

            try
            {
                using (dao_votesdb_context db = new dao_votesdb_context())
                {
                    List<Voting> vt = db.Votings.Where(x => jobids.Contains(x.JobID) && (x.Status == VoteStatusTypes.Completed || x.Status == VoteStatusTypes.Expired)).ToList();

                    res = _mapper.Map<List<Voting>, List<VotingDto>>(vt);
                }
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return res;
        }

        [Route("StartInformalVoting")]
        [HttpPost]
        public SimpleResponse StartInformalVoting([FromBody] VotingDto model)
        {
            SimpleResponse res = new SimpleResponse();

            try
            {
                using (dao_votesdb_context db = new dao_votesdb_context())
                {
                    if (db.Votings.Count(x => x.JobID == model.JobID && x.IsFormal == false && x.Type == VoteTypes.JobCompletion) > 0)
                    {
                        return new SimpleResponse() { Success = false, Message = "There is an existing informal voting process for this auction." };
                    }

                    //Start informal voting
                    Voting voting = new Voting();
                    voting.CreateDate = DateTime.Now;
                    voting.StartDate = model.StartDate;
                    voting.EndDate = model.EndDate;
                    voting.IsFormal = false;
                    voting.JobID = model.JobID;
                    voting.Status = Enums.VoteStatusTypes.Active;
                    voting.Type = Enums.VoteTypes.JobCompletion;
                    voting.PolicingRate = model.PolicingRate;
                    voting.StakedAgainst = 0;
                    voting.StakedFor = 0;
                    //Set quorum count based on DAO member count
                    voting.QuorumCount = model.QuorumCount;
                    db.Votings.Add(voting);
                    db.SaveChanges();

                    return new SimpleResponse() { Success = true, Message = "Informal voting started successfully.", Content = voting };
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
