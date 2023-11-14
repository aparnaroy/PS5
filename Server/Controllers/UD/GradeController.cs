﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
//using OCTOBER.EF.Data;
using OCTOBER.EF.Models;
using OCTOBER.Shared;
using Telerik.DataSource;
using Telerik.DataSource.Extensions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Linq.Dynamic.Core;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis;
using AutoMapper;
using OCTOBER.Server.Controllers.Base;
using OCTOBER.Shared.DTO;
using OCTOBER.EF;
using static System.Collections.Specialized.BitVector32;

namespace OCTOBER.Server.Controllers.UD
{
    [Route("api/[controller]")]
    [ApiController]

    public class GradeController : BaseController, GenericRestController<GradeDTO>
    {
        // Constructor
        public GradeController(OCTOBEROracleContext context,
            IHttpContextAccessor httpContextAccessor,
            IMemoryCache memoryCache)
        : base(context, httpContextAccessor)
        {
        }


        [HttpGet]
        [Route("Get")]
        public async Task<IActionResult> Get()
        {
            try
            {
                await _context.Database.BeginTransactionAsync();

                var result = await _context.Grades.Select(sp => new GradeDTO
                {
                    Comments = sp.Comments,
                    CreatedBy = sp.CreatedBy,
                    CreatedDate = sp.CreatedDate,
                    GradeCodeOccurrence = sp.GradeCodeOccurrence,
                    GradeTypeCode = sp.GradeTypeCode,
                    ModifiedBy = sp.ModifiedBy,
                    ModifiedDate = sp.ModifiedDate,
                    NumericGrade = sp.NumericGrade,
                    SchoolId = sp.SchoolId,
                    SectionId = sp.SectionId,
                    StudentId = sp.StudentId
                })
                .ToListAsync();
                await _context.Database.RollbackTransactionAsync();
                return Ok(result);
            }
            catch (Exception Dex)
            {
                await _context.Database.RollbackTransactionAsync();
                //List<OraError> DBErrors = ErrorHandling.TryDecodeDbUpdateException(Dex, _OraTranslateMsgs);
                return StatusCode(StatusCodes.Status417ExpectationFailed, "An Error has occurred");
            }
        }


        [HttpGet]
        [Route("Get/{SchoolID}/{SectionID}/{StudentID}/{GradeTypeCode}/{GradeCodeOccurrence}")]
        public async Task<IActionResult> Get(int SchoolID, int SectionID, int StudentID, string GradeTypeCode, int GradeCodeOccurrence)
        {
            try
            {
                await _context.Database.BeginTransactionAsync();

                GradeDTO? result = await _context.Grades
                    .Where(x => x.SchoolId == SchoolID)
                    .Where(x => x.SectionId == SectionID)
                    .Where(x => x.StudentId == StudentID)
                    .Where(x => x.GradeTypeCode.Equals(GradeTypeCode))
                    .Where(x => x.GradeCodeOccurrence == GradeCodeOccurrence)
                    .Select(sp => new GradeDTO
                    {
                        Comments = sp.Comments,
                        CreatedBy = sp.CreatedBy,
                        CreatedDate = sp.CreatedDate,
                        GradeCodeOccurrence = sp.GradeCodeOccurrence,
                        GradeTypeCode = sp.GradeTypeCode,
                        ModifiedBy = sp.ModifiedBy,
                        ModifiedDate = sp.ModifiedDate,
                        NumericGrade = sp.NumericGrade,
                        SchoolId = sp.SchoolId,
                        SectionId = sp.SectionId,
                        StudentId = sp.StudentId
                    })
                .SingleOrDefaultAsync();
                await _context.Database.RollbackTransactionAsync();
                return Ok(result);
            }
            catch (Exception Dex)
            {
                await _context.Database.RollbackTransactionAsync();
                //List<OraError> DBErrors = ErrorHandling.TryDecodeDbUpdateException(Dex, _OraTranslateMsgs);
                return StatusCode(StatusCodes.Status417ExpectationFailed, "An Error has occurred");
            }
        }


        [HttpPost]
        [Route("Post")]
        public async Task<IActionResult> Post([FromBody] GradeDTO _GradeDTO)
        {
            try
            {
                await _context.Database.BeginTransactionAsync();

                var itm = await _context.Grades
                    .Where(x => x.SchoolId == _GradeDTO.SchoolId)
                    .Where(x => x.SectionId == _GradeDTO.SectionId)
                    .Where(x => x.StudentId == _GradeDTO.StudentId)
                    .Where(x => x.GradeTypeCode.Equals(_GradeDTO.GradeTypeCode))
                    .Where(x => x.GradeCodeOccurrence == _GradeDTO.GradeCodeOccurrence)
                    .FirstOrDefaultAsync();

                if (itm == null)
                {
                    Grade g = new Grade
                    {
                        // All columns w/o createds and modifieds
                        Comments = _GradeDTO.Comments,
                        GradeCodeOccurrence = _GradeDTO.GradeCodeOccurrence,
                        GradeTypeCode = _GradeDTO.GradeTypeCode,
                        NumericGrade = _GradeDTO.NumericGrade,
                        SchoolId = _GradeDTO.SchoolId,
                        SectionId = _GradeDTO.SectionId,
                        StudentId = _GradeDTO.StudentId
                    };
                    _context.Grades.Add(g);
                    await _context.SaveChangesAsync();
                    await _context.Database.CommitTransactionAsync();
                }
                return Ok();
            }
            catch (Exception Dex)
            {
                await _context.Database.RollbackTransactionAsync();
                //List<OraError> DBErrors = ErrorHandling.TryDecodeDbUpdateException(Dex, _OraTranslateMsgs);
                return StatusCode(StatusCodes.Status417ExpectationFailed, "An Error has occurred");
            }
        }


        [HttpPut]
        [Route("Put")]
        public async Task<IActionResult> Put([FromBody] GradeDTO _GradeDTO)
        {
            try
            {
                await _context.Database.BeginTransactionAsync();

                var itm = await _context.Grades
                    .Where(x => x.SchoolId == _GradeDTO.SchoolId)
                    .Where(x => x.SectionId == _GradeDTO.SectionId)
                    .Where(x => x.StudentId == _GradeDTO.StudentId)
                    .Where(x => x.GradeTypeCode.Equals(_GradeDTO.GradeTypeCode))
                    .Where(x => x.GradeCodeOccurrence == _GradeDTO.GradeCodeOccurrence)
                    .FirstOrDefaultAsync();

                // All columns w/o PKs, createds, and modifieds
                itm.Comments = _GradeDTO.Comments;
                itm.NumericGrade = _GradeDTO.NumericGrade;

                _context.Grades.Update(itm);
                await _context.SaveChangesAsync();
                await _context.Database.CommitTransactionAsync();

                return Ok();
            }
            catch (Exception Dex)
            {
                await _context.Database.RollbackTransactionAsync();
                //List<OraError> DBErrors = ErrorHandling.TryDecodeDbUpdateException(Dex, _OraTranslateMsgs);
                return StatusCode(StatusCodes.Status417ExpectationFailed, "An Error has occurred");
            }
        }


        [HttpDelete]
        [Route("Delete/{SchoolID}/{SectionID}/{StudentID}/{GradeTypeCode}/{GradeCodeOccurrence}")]
        public async Task<IActionResult> Delete(int SchoolID, int SectionID, int StudentID, string GradeTypeCode, int GradeCodeOccurrence)
        {
            try
            {
                await _context.Database.BeginTransactionAsync();

                var itm = await _context.Grades
                    .Where(x => x.SchoolId == SchoolID)
                    .Where(x => x.SectionId == SectionID)
                    .Where(x => x.StudentId == StudentID)
                    .Where(x => x.GradeTypeCode.Equals(GradeTypeCode))
                    .Where(x => x.GradeCodeOccurrence == GradeCodeOccurrence)
                    .FirstOrDefaultAsync();

                if (itm != null)
                {
                    _context.Grades.Remove(itm);
                }
                await _context.SaveChangesAsync();
                await _context.Database.CommitTransactionAsync();

                return Ok();
            }
            catch (Exception Dex)
            {
                await _context.Database.RollbackTransactionAsync();
                //List<OraError> DBErrors = ErrorHandling.TryDecodeDbUpdateException(Dex, _OraTranslateMsgs);
                return StatusCode(StatusCodes.Status417ExpectationFailed, "An Error has occurred");
            }
        }


        public Task<IActionResult> Get(int KeyVal)
        {
            throw new NotImplementedException();
        }


        public Task<IActionResult> Delete(int KeyVal)
        {
            throw new NotImplementedException();
        }
    }
}
