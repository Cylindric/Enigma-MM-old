using System;

namespace EnigmaMM.Interfaces
{
    /// <summary>
    /// Provides an interface for Scheduled Tasks.
    /// </summary>
    public interface IScheduleTask
    {
        /// <summary>
        /// Gets or sets the name for this task.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the command to run when the task executes.
        /// </summary>
        string Command { get; set; }

        /// <summary>
        /// Gets the time this task is next set to run.
        /// </summary>
        DateTime NextRun { get; }

        /// <summary>
        /// Gets or sets the Days part of the task's schedule.
        /// </summary>
        string RunDays { get; set; }

        /// <summary>
        /// Gets or sets the Hours part of the task's schedule.
        /// </summary>
        string RunHours { get; set; }

        /// <summary>
        /// Gets or sets the Minutes part of the task's schedule.
        /// </summary>
        string RunMinutes { get; set; }

        /// <summary>
        /// Calculates the next time this task is due to run, after the current time.
        /// </summary>
        /// <returns>The calculated next run time.</returns>
        DateTime CalculateNextRunTime();

        /// <summary>
        /// Calculates the next time this task is due to run, after the specified time.
        /// </summary>
        /// <param name="start">The time to start calculating from.</param>
        /// <returns>The calculated next run time.</returns>
        DateTime CalculateNextRunTime(DateTime start);
    }
}
