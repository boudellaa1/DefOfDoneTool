﻿// <copyright file="TaskChecklistController.cs" company="Skyline Communications">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace DoneTool.Controllers
{
    using System;
    using AutoMapper;
    using DoneTool.Models.Domain;
    using DoneTool.Models.DTO;
    using DoneTool.Repositories.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Initializes a new instance of the <see cref="TaskChecklistController"/> class.
    /// </summary>
    /// <param name="taskChecklistRepository">The repository to manage task checklists.</param>
    /// <param name="mapper">The mapper for mapping between domain models and DTOs.</param>
    [Route("api/[controller]")]
    [ApiController]
    public class TaskChecklistController(ITaskChecklistRepository taskChecklistRepository, IMapper mapper)
        : ControllerBase
    {
        /// <summary>
        /// Gets all task checklists.
        /// </summary>
        /// <returns>A list of <see cref="GetTaskChecklistsDTO"/> representing all task checklists.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetTaskChecklistsDTO>>> GetTaskChecklists()
        {
            var taskChecklists = await taskChecklistRepository.GetAllTaskChecklists();
            var getTaskChecklistsDTOs = mapper.Map<IEnumerable<GetTaskChecklistsDTO>>(taskChecklists);
            return this.Ok(getTaskChecklistsDTOs);
        }

        /// <summary>
        /// Gets a specific task checklist by ID.
        /// </summary>
        /// <param name="id">The ID of the task checklist.</param>
        /// <returns>The <see cref="GetTaskChecklistDTO"/> representing the requested task checklist.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<GetTaskChecklistDTO>> GetTaskChecklist(Guid id)
        {
            var taskChecklist = await taskChecklistRepository.GetTaskChecklistById(id);

            if (taskChecklist == null)
            {
                return this.NotFound();
            }

            var getTaskChecklistDTO = mapper.Map<GetTaskChecklistDTO>(taskChecklist);
            return this.Ok(getTaskChecklistDTO);
        }

        /// <summary>
        /// Creates a new task checklist.
        /// </summary>
        /// <param name="postTaskChecklistDTO">The DTO containing the details of the task checklist to create.</param>
        /// <returns>The created <see cref="PostTaskChecklistDTO"/> with its ID.</returns>
        [HttpPost]
        public async Task<ActionResult<PostTaskChecklistDTO>> PostTaskChecklist(PostTaskChecklistDTO postTaskChecklistDTO)
        {
            var taskChecklist = mapper.Map<TaskChecklist>(postTaskChecklistDTO);
            await taskChecklistRepository.AddTaskChecklist(taskChecklist);

            var createdPostTaskChecklistDTO = mapper.Map<PostTaskChecklistDTO>(taskChecklist);
            return this.CreatedAtAction(nameof(this.GetTaskChecklist), new { id = taskChecklist.ID }, createdPostTaskChecklistDTO);
        }

        /// <summary>
        /// Updates an existing task checklist.
        /// </summary>
        /// <param name="id">The ID of the task checklist to update.</param>
        /// <param name="taskChecklistDTO">The DTO containing the updated details of the task checklist.</param>
        /// <returns>
        /// An IActionResult that represents the result of the update operation.
        /// Returns:
        /// - 200 OK with the updated LastUpdated timestamp if the operation is successful.
        /// - 404 Not Found if the task checklist with the specified ID does not exist.
        /// - 409 Conflict if the task checklist has been updated by someone else since it was last retrieved.
        /// </returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTaskChecklist(Guid id, PutTaskChecklistDTO taskChecklistDTO)
        {
            var existingTaskChecklist = await taskChecklistRepository.GetTaskChecklistById(id);
            if (existingTaskChecklist == null)
            {
                return this.NotFound();
            }

            if (taskChecklistDTO.LastUpdated.ToString("yyyy-MM-ddTHH:mm:ssZ")
                != existingTaskChecklist.LastUpdated.ToString("yyyy-MM-ddTHH:mm:ssZ"))
            {
                return this.StatusCode(409, "The data has been updated by someone else. Please reload and try again.");
            }

            existingTaskChecklist.Status = taskChecklistDTO.Status;
            existingTaskChecklist.Comment = taskChecklistDTO.Comment;
            existingTaskChecklist.Guard = taskChecklistDTO.Guard;
            existingTaskChecklist.LastUpdated = DateTime.UtcNow;

            await taskChecklistRepository.UpdateTaskChecklist(existingTaskChecklist);

            var response = new { existingTaskChecklist.LastUpdated };

            return this.Ok(response);
        }

        /// <summary>
        /// Deletes a task checklist.
        /// </summary>
        /// <param name="id">The ID of the task checklist to delete.</param>
        /// <returns>A status indicating the result of the operation.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTaskChecklist(Guid id)
        {
            var taskChecklist = await taskChecklistRepository.GetTaskChecklistById(id);
            if (taskChecklist == null)
            {
                return this.NotFound();
            }

            await taskChecklistRepository.DeleteTaskChecklist(id);

            return this.NoContent();
        }

        /// <summary>
        /// Duplicates a specific task checklist.
        /// </summary>
        /// <param name="id">The ID of the task checklist to duplicate.</param>
        /// <returns>The duplicated <see cref="GetTaskChecklistDTO"/>.</returns>
        [HttpPost("{id}/duplicate")]
        public async Task<ActionResult<GetTaskChecklistDTO>> DuplicateTaskChecklist(Guid id)
        {
            var existingTaskChecklist = await taskChecklistRepository.GetTaskChecklistById(id);
            if (existingTaskChecklist == null)
            {
                return this.NotFound();
            }

            var duplicateTaskChecklist = new TaskChecklist
            {
                ID = Guid.NewGuid(),
                TaskID = existingTaskChecklist.TaskID,
                TaskChecksID = existingTaskChecklist.TaskChecksID,
                Status = existingTaskChecklist.Status,
                Guard = existingTaskChecklist.Guard,
                Comment = existingTaskChecklist.Comment,
                LastUpdated = DateTime.UtcNow,
                OriginalTaskChecklistID = id,
            };

            await taskChecklistRepository.AddTaskChecklist(duplicateTaskChecklist);

            var getTaskChecklistDTO = mapper.Map<GetTaskChecklistDTO>(duplicateTaskChecklist);
            return this.Ok(new { id = duplicateTaskChecklist.ID });
        }

        /// <summary>
        /// Deletes a duplicated task checklist.
        /// </summary>
        /// <param name="id">The ID of the task checklist to delete.</param>
        /// <returns>A status indicating the result of the operation.</returns>
        [HttpDelete("{id}/duplicate")]
        public async Task<IActionResult> DeleteDuplicateTaskChecklist(Guid id)
        {
            var taskChecklist = await taskChecklistRepository.GetTaskChecklistById(id);
            if (taskChecklist == null || taskChecklist.OriginalTaskChecklistID == null)
            {
                return this.NotFound("Cannot delete non-duplicate task checklist.");
            }

            await taskChecklistRepository.DeleteTaskChecklist(id);

            return this.NoContent();
        }

        /// <summary>
        /// Gets the duplicate of a specific task checklist.
        /// </summary>
        /// <param name="id">The ID of the original task checklist.</param>
        /// <returns>The duplicated <see cref="GetTaskChecklistDTO"/>.</returns>
        [HttpGet("{id}/duplicate")]
        public async Task<ActionResult<GetTaskChecklistDTO>> GetDuplicateTaskChecklist(Guid id)
        {
            var taskChecklists = await taskChecklistRepository.GetAllTaskChecklists();

            var duplicateTaskChecklist = taskChecklists.FirstOrDefault(tc => tc.OriginalTaskChecklistID == id);

            if (duplicateTaskChecklist == null)
            {
                return this.NotFound("No duplicate found for the specified task checklist.");
            }

            var getTaskChecklistDTO = mapper.Map<GetTaskChecklistDTO>(duplicateTaskChecklist);

            return this.Ok(getTaskChecklistDTO);
        }
    }
}