using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.Http.OData;
using System.Web.Http.OData.Routing;
using Dotnetconf_DataAccess;

namespace Dotnetconf_DataAccess.Controllers
{
    /*
    The WebApiConfig class may require additional changes to add a route for this controller. Merge these statements into the Register method of the WebApiConfig class as applicable. Note that OData URLs are case sensitive.

    using System.Web.Http.OData.Builder;
    using System.Web.Http.OData.Extensions;
    using Dotnetconf_DataAccess;
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<Attendes>("Attendes");
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class AttendesController : ODataController
    {
        private dotnetconf db = new dotnetconf();

        // GET: odata/Attendes
        [EnableQuery]
        public IQueryable<Attendes> GetAttendes()
        {
            return db.MyEntities;
        }

        // GET: odata/Attendes(5)
        [EnableQuery]
        public SingleResult<Attendes> GetAttendes([FromODataUri] int key)
        {
            return SingleResult.Create(db.MyEntities.Where(attendes => attendes.Id == key));
        }

        // PUT: odata/Attendes(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<Attendes> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Attendes attendes = await db.MyEntities.FindAsync(key);
            if (attendes == null)
            {
                return NotFound();
            }

            patch.Put(attendes);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AttendesExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(attendes);
        }

        // POST: odata/Attendes
        public async Task<IHttpActionResult> Post(Attendes attendes)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.MyEntities.Add(attendes);
            await db.SaveChangesAsync();

            return Created(attendes);
        }

        // PATCH: odata/Attendes(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<Attendes> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Attendes attendes = await db.MyEntities.FindAsync(key);
            if (attendes == null)
            {
                return NotFound();
            }

            patch.Patch(attendes);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AttendesExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(attendes);
        }

        // DELETE: odata/Attendes(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            Attendes attendes = await db.MyEntities.FindAsync(key);
            if (attendes == null)
            {
                return NotFound();
            }

            db.MyEntities.Remove(attendes);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AttendesExists(int key)
        {
            return db.MyEntities.Count(e => e.Id == key) > 0;
        }
    }
}
