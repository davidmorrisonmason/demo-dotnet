using Demo.Api.Dto;
using Demo.DomainServices.Interface.Command;
using Demo.DomainServices.Interface.Orchestration;
using Demo.Model.Domain;
using Demo.Model.Validation;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Api.Controllers
{
    /// <summary>
    /// Base controller class
    /// </summary>
    public class ApiController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger _logger;

        /// <summary>
        /// The mediator 
        /// </summary>
        protected IMediator Mediator => _mediator;

        /// <summary>
        /// The logger
        /// </summary>
        protected ILogger Logger => _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger">The logger</param>
        /// <param name="mediator">The mediator</param>
        public ApiController(
            ILogger logger,
            IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        /// <summary>
        /// Executes a command
        /// </summary>
        /// <typeparam name="TCommand">The type of command to execute</typeparam>
        /// <param name="command">The command to execute</param>
        /// <returns>NoContent result</returns>
        protected async Task<ActionResult> ExecuteCommand<TCommand>(TCommand command)
        where TCommand : IRequest
        {
            try
            {
                _logger.LogDebug($"Sending {typeof(TCommand).Name} command to mediator");

                await Mediator.Send(command);

                _logger.LogDebug($"Command returned successfully");

                return NoContent();
            }
            catch (Exception ex)
            {
                return ToErrorResult(ex);
            }
        }

        /// <summary>
        /// Executes a command and returns a single DTO result
        /// </summary>
        /// <typeparam name="TCommand">The type of command</typeparam>
        /// <typeparam name="TResult">The type of result</typeparam>
        /// <typeparam name="TResultDto">The type of result DTO</typeparam>
        /// <param name="command">The command to execute</param>
        /// <returns>an OK result with the created DTO or an error result</returns>
        protected async Task<ActionResult> ExecuteCommandWithResult<TCommand, TResult, TResultDto>(TCommand command)
        where TCommand : IRequest<TResult>
        where TResult : class
        {
            try
            {
                _logger.LogDebug($"Sending {typeof(TCommand).Name} command to mediator");

                var result = await Mediator.Send(command);

                _logger.LogDebug($"Command returned successfully");

                return Ok(result.Adapt<TResultDto>());
            }
            catch (Exception ex)
            {
                return ToErrorResult(ex);
            }
        }

        protected async Task<ActionResult> ExecuteQuery<TQuery, TEntity, TResponseDto>(TQuery query)
            where TQuery : IRequest<IEnumerable<TEntity>>
            where TEntity : class
        {
            try
            {
                _logger.LogDebug($"Sending {typeof(TQuery).Name} query to mediator");

                var result = ToGetListResult<TResponseDto>(await Mediator.Send(query));

                _logger.LogDebug($"Query returned successfully");

                return result;
            }
            catch (Exception ex)
            {
                return ToErrorResult(ex);
            }
        }

        protected async Task<ActionResult> ExecuteQuerySingle<TQuery, TEntity, TResponseDto>(
            TQuery query)
            where TQuery : IRequest<TEntity?>
        {
            try
            {
                _logger.LogDebug($"Sending {typeof(TQuery).Name} query to mediator");

                var result = ToGetResult<TResponseDto>(await Mediator.Send(query));

                _logger.LogDebug($"Query returned successfully");

                return result;
            }
            catch (Exception ex)
            {
                return ToErrorResult(ex);
            }
        }

        protected async Task<ActionResult> ExecutePostCommand<TCommand, TEntity>(string actionName, TCommand command) where TEntity : DomainObject
            where TCommand : Command<TEntity>

        {
            try
            {
                _logger.LogDebug($"Sending {typeof(TCommand).Name} command  to mediator");

                var createdEntity = await Mediator.Send(command);

                var result = ToPostResult(actionName, createdEntity);

                _logger.LogDebug($"Command returned successfully");

                return result;
            }
            catch (Exception ex)
            {
                return ToErrorResult(ex);
            }
        }

        protected async Task<ActionResult> ExecutePutCommand<TCommand, TEntity>(TCommand command)
            where TCommand : Command, IRequest
        {
            try
            {
                _logger.LogDebug($"Sending {typeof(TCommand).Name} command  to mediator");

                await Mediator.Send(command);

                var result = ToPutResult();

                _logger.LogDebug($"Command returned successfully");

                return result;
            }
            catch (Exception ex)
            {
                return ToErrorResult(ex);
            }
        }

        protected async Task<ActionResult> ExecuteDeleteCommand<TCommand, TEntity>(TCommand command)
            where TCommand : Command, IRequest
        {
            try
            {
                _logger.LogDebug($"Sending {typeof(TCommand).Name} command  to mediator");

                await Mediator.Send(command);

                var result = ToDeleteResult();

                _logger.LogDebug($"Command returned successfully");

                return result;
            }
            catch (Exception ex)
            {
                return ToErrorResult(ex);
            }
        }

        private ActionResult ToGetListResult<TDto>(IEnumerable<object> list)
        {
            return Ok(list.Select(x => x.Adapt<TDto>()).ToList());
        }

        private ActionResult ToGetResult<TDto>(object? sourceObject)
        {
            return sourceObject == null ? NotFound() : Ok(sourceObject.Adapt<TDto>());
        }

        private ActionResult ToPostResult<TEntity>(string actionName, TEntity sourceObject) where TEntity : DomainObject
        {
            var routeValues = new { id = sourceObject.Id };
            return CreatedAtAction(actionName, routeValues, sourceObject.Adapt<EntityDto>());
        }

        private ActionResult ToDeleteResult()
        {
            return NoContent();
        }

        private ActionResult ToPutResult()
        {
            return NoContent();
        }

        private ActionResult ToErrorResult(Exception ex)
        {
            if (ex is EntityNotFoundException)
            {
                _logger.LogDebug("EntityNotFound exception handled");
                return NotFound();
            }

            // otherwise rethrow and let the plumbing handle it
            _logger.LogError(ex.ToString());

            throw (ex);
        }
    }
}
