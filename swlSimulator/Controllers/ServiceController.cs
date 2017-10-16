﻿using Microsoft.AspNetCore.Mvc;
using swlSimulator.api;
using swlSimulator.api.Combat;
using swlSimulator.api.Utilities;
using swlSimulator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace swlSimulator.Controllers
{

    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("/api/values")]
    public class ServiceController : Controller
    {
        private List<FightResult> _iterationFightResults;
        private Settings _settings = new Settings();

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Settings settings)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _settings = settings;

            // Simulation async
            var result = await Task.Run(() => StartSimulation());
            if (!result)
            {
                // Simulation failed
                //return View(settings);
            }

            var report = new Report();

            result = await Task.Run(() => report.GenerateReportData(_iterationFightResults, settings));
            if (!result)
            {
                // Report generation failed
                //return View(settings);
            }

            //return View("Results", report);
            return Ok(report);
        }

        private bool StartSimulation()
        {
            var res = false;

            try
            {
                var engine = new Engine(_settings);
                _iterationFightResults = engine.StartIterations();

                res = true;
            }
            catch (Exception e) when (!Helper.Env.Debugging)
            {
                // TODO: Log exception and show to user
            }

            return res;
        }
    }
}
