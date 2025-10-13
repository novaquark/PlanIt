using GraphQL;
using P4PlanLib.Model;
using System.Collections.Concurrent;

namespace P4PlanLib
{
    public class P4PlanDummyClient : IP4PlanClient
    {
        private readonly string _connectedUser = "dev-user@example.com";
        private readonly string _connectedUserName = "Development User";

        private readonly ConcurrentDictionary<string, Item> _items = new();
        private readonly ConcurrentDictionary<string, List<Comment>> _comments = new();
        private readonly ConcurrentDictionary<string, List<Item>> _children = new();

        public P4PlanDummyClient()
        {
            GenerateFakeData();
        }

        private void GenerateFakeData()
        {
            var random = new Random(42); // Fixed seed for consistent data

            // Generate Epics
            for (int i = 1; i <= 8; i++)
            {
                var id = $"EPIC-{i:D3}";
                var assignedUser = i <= 3 ? "Development User" : GetRandomDeveloper(random); // First 3 epics assigned to dev user
                var item = new Item
                {
                    Id = id,
                    Name = $"Epic {i}: {GetEpicName(i)}",
                    Description = GetEpicDescription(i),
                    Status = GetRandomStatus(random),
                    Type = ItemType.Epic,
                    AssignedTo = [new AssignedTo { User = new User() { Name = assignedUser } }],
                    Priority = GetRandomPriority(random),
                    Severity = GetRandomSeverity(random),
                    CommittedTo = GetRandomSprint(random),
                    EstimatedDays = random.Next(20, 60),
                    WorkRemaining = random.Next(5, 40),
                    IndentationLevel = 0,
                    Rank = i,
                    Hyperlink = $"https://novaquark.hansoft.cloud/task/3767a0ec/{id}",
                    ItemLink = $"https://novaquark.hansoft.cloud/task/3767a0ec/{id}",
                    SubprojectPath = GetRandomTeam(random)
                };

                _items[id] = item;
                GenerateCommentsForItem(id, item, random);
                GenerateChildrenForEpic(id, item, random);
            }

            // Generate Stories
            for (int i = 1; i <= 25; i++)
            {
                var id = $"STORY-{i:D3}";
                var assignedUser = i <= 8 ? "Development User" : GetRandomDeveloper(random); // First 8 stories assigned to dev user
                var item = new Item
                {
                    Id = id,
                    Name = $"Story {i}: {GetStoryName(i)}",
                    Description = GetStoryDescription(i),
                    Status = GetRandomStatus(random),
                    Type = ItemType.Story,
                    AssignedTo = [new AssignedTo { User = new User() { Name = assignedUser } }],
                    Priority = GetRandomPriority(random),
                    Severity = GetRandomSeverity(random),
                    CommittedTo = GetRandomSprint(random),
                    EstimatedDays = random.Next(3, 15),
                    WorkRemaining = random.Next(0, 12),
                    IndentationLevel = random.Next(0, 2),
                    Rank = 50 + i,
                    Hyperlink = $"https://novaquark.hansoft.cloud/task/3767a0ec/{id}",
                    ItemLink = $"https://novaquark.hansoft.cloud/task/3767a0ec/{id}",
                    SubprojectPath = GetRandomTeam(random)
                };

                _items[id] = item;
                GenerateCommentsForItem(id, item, random);
                GenerateChildrenForStory(id, item, random);
            }

            // Generate Tasks
            for (int i = 1; i <= 40; i++)
            {
                var id = $"TASK-{i:D3}";
                var assignedUser = i <= 15 ? "Development User" : GetRandomDeveloper(random); // First 15 tasks assigned to dev user
                var item = new Item
                {
                    Id = id,
                    Name = $"Task {i}: {GetTaskName(i)}",
                    Description = GetTaskDescription(i),
                    Status = GetRandomStatus(random),
                    Type = ItemType.Task,
                    AssignedTo = [new AssignedTo { User = new User() { Name = assignedUser } }],
                    Priority = GetRandomPriority(random),
                    Severity = GetRandomSeverity(random),
                    CommittedTo = GetRandomSprint(random, 1), // Skip Backlog
                    EstimatedDays = random.Next(1, 8),
                    WorkRemaining = random.Next(0, 6),
                    IndentationLevel = random.Next(1, 4),
                    Rank = 100 + i,
                    Hyperlink = $"https://novaquark.hansoft.cloud/task/3767a0ec/{id}",
                    ItemLink = $"https://novaquark.hansoft.cloud/task/3767a0ec/{id}",
                    SubprojectPath = GetRandomTeam(random)
                };

                _items[id] = item;
                GenerateCommentsForItem(id, item, random);
            }

            // Generate Bugs
            for (int i = 1; i <= 20; i++)
            {
                var id = $"BUG-{i:D3}";
                var assignedUser = i <= 6 ? "Development User" : GetRandomDeveloper(random); // First 6 bugs assigned to dev user
                var item = new Item
                {
                    Id = id,
                    Name = $"Bug {i}: {GetBugName(i)}",
                    Description = GetBugDescription(i),
                    Status = GetRandomStatus(random),
                    Type = ItemType.Bug,
                    AssignedTo = [new AssignedTo { User = new User() { Name = assignedUser } }],
                    Priority = GetRandomPriority(random),
                    Severity = GetRandomSeverity(random),
                    CommittedTo = GetRandomSprint(random),
                    EstimatedDays = random.Next(1, 10),
                    WorkRemaining = random.Next(0, 8),
                    IndentationLevel = 0,
                    Rank = 200 + i,
                    Hyperlink = $"https://novaquark.hansoft.cloud/task/3767a0ec/{id}",
                    ItemLink = $"https://novaquark.hansoft.cloud/task/3767a0ec/{id}",
                    SubprojectPath = GetRandomTeam(random)
                };

                _items[id] = item;
                GenerateCommentsForItem(id, item, random);
            }

            // Generate Critical Bugs (Showstoppers)
            for (int i = 1; i <= 5; i++)
            {
                var id = $"SHOW-{i:D3}";
                var assignedUser = i <= 2 ? "Development User" : GetRandomDeveloper(random); // First 2 showstoppers assigned to dev user
                var item = new Item
                {
                    Id = id,
                    Name = $"Critical Bug {i}: {GetShowstopperName(i)}",
                    Description = GetShowstopperDescription(i),
                    Status = GetRandomStatus(random, 0, 3), // Only New, To Do, In Progress
                    Type = ItemType.Showstopper,
                    AssignedTo = [new AssignedTo { User = new User() { Name = assignedUser } }],
                    Priority = "veryHigh",
                    Severity = "Critical",
                    CommittedTo = GetRandomSprint(random, 1), // Skip Backlog
                    EstimatedDays = random.Next(2, 8),
                    WorkRemaining = random.Next(1, 6),
                    IndentationLevel = 0,
                    Rank = 300 + i,
                    Hyperlink = $"https://novaquark.hansoft.cloud/task/3767a0ec/{id}",
                    ItemLink = $"https://novaquark.hansoft.cloud/task/3767a0ec/{id}",
                    SubprojectPath = GetRandomTeam(random)
                };

                _items[id] = item;
                GenerateCommentsForItem(id, item, random);
            }
        }

        private void GenerateChildrenForEpic(string epicId, Item epic, Random random)
        {
            var childCount = random.Next(3, 8);
            var children = new List<Item>();

            for (int i = 1; i <= childCount; i++)
            {
                var child = new Item
                {
                    Id = $"{epicId}-ST{i:D2}",
                    Name = $"Story {i} for {epic.Name}",
                    Description = $"Implementation story for {epic.Name}",
                    Status = GetRandomStatus(random),
                    Type = ItemType.Story,
                    AssignedTo = [new AssignedTo { User = new User() { Name = GetRandomDeveloper(random) } }],
                    CommittedTo = epic.CommittedTo,
                    Priority = GetRandomPriority(random),
                    EstimatedDays = random.Next(5, 20),
                    WorkRemaining = random.Next(0, 15),
                    IndentationLevel = 1,
                    Rank = i,
                    SubprojectPath = epic.SubprojectPath
                };

                children.Add(child);
                _items[child.Id] = child;
                GenerateCommentsForItem(child.Id, child, random);
                GenerateChildrenForStory(child.Id, child, random);
            }

            _children[epicId] = children;
        }

        private void GenerateChildrenForStory(string storyId, Item story, Random random)
        {
            var childCount = random.Next(2, 6);
            var children = new List<Item>();

            for (int i = 1; i <= childCount; i++)
            {
                var child = new Item
                {
                    Id = $"{storyId}-T{i:D2}",
                    Name = $"Task {i} for {story.Name}",
                    Description = $"Implementation task for {story.Name}",
                    Status = GetRandomStatus(random),
                    Type = ItemType.Task,
                    AssignedTo = [new AssignedTo { User = new User() { Name = GetRandomDeveloper(random) } }],
                    CommittedTo = story.CommittedTo,
                    Priority = GetRandomPriority(random),
                    EstimatedDays = random.Next(1, 5),
                    WorkRemaining = random.Next(0, 3),
                    IndentationLevel = story.IndentationLevel + 1,
                    Rank = i,
                    SubprojectPath = story.SubprojectPath
                };

                children.Add(child);
                _items[child.Id] = child;
                GenerateCommentsForItem(child.Id, child, random);
            }

            _children[storyId] = children;
        }

        private void GenerateCommentsForItem(string itemId, Item item, Random random)
        {
            var commentCount = random.Next(2, 8);
            var comments = new List<Comment>();

            for (int i = 1; i <= commentCount; i++)
            {
                var comment = new Comment
                {
                    Id = $"{itemId}-C{i}",
                    Text = GetCommentText(item, i, random),
                    CreatedAt = DateTime.Now.AddDays(-random.Next(0, 30)),
                    CreatedBy = GetRandomDeveloper(random)
                };
                comments.Add(comment);
            }

            _comments[itemId] = comments;
        }

        // Helper methods for random data generation
        private string GetRandomStatus(Random random, int startIndex = 0, int? endIndex = null)
        {
            var statuses = new[] { "New", "To Do", "In Progress", "In Review", "Done", "Blocked", "On Hold" };
            var end = endIndex ?? statuses.Length;
            return statuses[random.Next(startIndex, end)];
        }

        private string GetRandomPriority(Random random)
        {
            var priorities = new[] { "veryLow", "low", "medium", "high", "veryHigh" };
            return priorities[random.Next(priorities.Length)];
        }

        private string GetRandomSeverity(Random random)
        {
            var severities = new[] { "Low", "Medium", "High", "Critical" };
            return severities[random.Next(severities.Length)];
        }

        private Sprint GetRandomSprint(Random random, int startIndex = 0)
        {
            var sprintNames = new[] { "Backlog", "Sprint 1", "Sprint 2", "Sprint 3", "Sprint 4", "Sprint 5", "Sprint 6" };
            var sprintName = sprintNames[random.Next(startIndex, sprintNames.Length)];

            return new Sprint
            {
                Name = sprintName,
                Id = $"SPRINT-{sprintName.Replace(" ", "")}",
                StartDate = GetSprintStartDate(sprintName),
                EndDate = GetSprintEndDate(sprintName)
            };
        }

        private DateTime? GetSprintStartDate(string sprintName)
        {
            return sprintName switch
            {
                "Backlog" => null,
                "Sprint 1" => new DateTime(2024, 10, 15), // Current sprint
                "Sprint 2" => new DateTime(2024, 11, 1),  // Future sprint
                "Sprint 3" => new DateTime(2024, 11, 15), // Future sprint
                "Sprint 4" => new DateTime(2024, 12, 1),  // Future sprint
                "Sprint 5" => new DateTime(2024, 12, 15), // Future sprint
                "Sprint 6" => new DateTime(2025, 1, 1),   // Future sprint
                _ => null
            };
        }

        private DateTime? GetSprintEndDate(string sprintName)
        {
            return sprintName switch
            {
                "Backlog" => null,
                "Sprint 1" => new DateTime(2024, 10, 31), // Current sprint
                "Sprint 2" => new DateTime(2024, 11, 14), // Future sprint
                "Sprint 3" => new DateTime(2024, 11, 30), // Future sprint
                "Sprint 4" => new DateTime(2024, 12, 14), // Future sprint
                "Sprint 5" => new DateTime(2024, 12, 31), // Future sprint
                "Sprint 6" => new DateTime(2025, 1, 15),  // Future sprint
                _ => null
            };
        }

        private string GetRandomDeveloper(Random random)
        {
            var developers = new[] {
                "Development User", "alice@example.com", "bob@example.com", "charlie@example.com",
                "diana@example.com", "eve@example.com", "frank@example.com",
                "grace@example.com", "henry@example.com", "iris@example.com"
            };
            return developers[random.Next(developers.Length)];
        }

        private string GetRandomTeam(Random random)
        {
            var teams = new[] {
                "S32 Tools", "S32 Design", "Core Platform", "Mobile Team",
                "Infrastructure", "Security Team", "Data Analytics", "QA Team"
            };
            return teams[random.Next(teams.Length)];
        }

        // Data generation methods
        private string GetEpicName(int index)
        {
            var names = new[] {
                "Implement Comprehensive User Management System",
                "Build Real-time Analytics Dashboard",
                "Create Advanced Reporting Engine",
                "Develop Mobile Application Suite",
                "Establish CI/CD Pipeline Infrastructure",
                "Implement Enterprise Security Framework",
                "Create Data Integration Platform",
                "Build Performance Monitoring System"
            };
            return names[index - 1];
        }

        private string GetEpicDescription(int index)
        {
            var descriptions = new[] {
                "Complete overhaul of user management including role-based access control, user provisioning, and audit logging.",
                "Real-time analytics dashboard with interactive charts, customizable widgets, and automated data refresh.",
                "Advanced reporting engine supporting multiple formats (PDF, Excel, CSV), scheduled reports, and custom report builder.",
                "Native mobile applications for iOS and Android platforms with offline capabilities and push notifications.",
                "Complete CI/CD pipeline with automated testing, deployment automation, and environment management.",
                "Enterprise-grade security framework including encryption, authentication, authorization, and compliance features.",
                "Data integration platform supporting multiple data sources, ETL processes, and real-time data synchronization.",
                "Comprehensive performance monitoring with metrics collection, alerting, and performance optimization recommendations."
            };
            return descriptions[index - 1];
        }

        private string GetStoryName(int index)
        {
            var names = new[] {
                "User Authentication with OAuth2",
                "Role-based Permission System",
                "User Profile Management",
                "Dashboard Widget Framework",
                "Real-time Data Streaming",
                "Chart Component Library",
                "Report Template Engine",
                "Export Functionality",
                "Mobile App Navigation",
                "Offline Data Sync",
                "Push Notification Service",
                "API Rate Limiting",
                "Database Query Optimization",
                "Security Audit Logging",
                "Performance Metrics Collection",
                "Automated Testing Suite",
                "Deployment Pipeline",
                "Environment Configuration",
                "Data Backup System",
                "User Activity Tracking",
                "Search and Filtering",
                "Bulk Operations",
                "Email Notifications",
                "System Health Monitoring",
                "User Preferences Management"
            };
            return names[index - 1];
        }

        private string GetStoryDescription(int index)
        {
            var descriptions = new[] {
                "Implement OAuth2 authentication flow with support for multiple providers (Google, Microsoft, GitHub).",
                "Create flexible role-based permission system allowing administrators to define custom roles with granular permissions.",
                "Comprehensive user profile management including personal information, preferences, avatar upload, and profile customization.",
                "Extensible dashboard widget framework allowing users to create, customize, and arrange widgets for personalized dashboard experience.",
                "Real-time data streaming infrastructure using WebSockets for live updates across dashboard components and user interfaces.",
                "Reusable chart component library supporting various chart types (line, bar, pie, scatter) with interactive features and responsive design.",
                "Flexible report template engine allowing users to create custom report layouts, define data sources, and set formatting rules.",
                "Multi-format export functionality supporting PDF, Excel, CSV, and JSON formats with customizable export options and batch processing.",
                "Intuitive mobile app navigation with bottom tabs, side drawer, and gesture-based interactions for optimal mobile user experience.",
                "Offline data synchronization system allowing users to work without internet connection and sync changes when connectivity is restored.",
                "Push notification service for mobile apps with support for different notification types, user preferences, and delivery tracking.",
                "API rate limiting implementation with configurable limits, user quotas, and graceful degradation for API abuse prevention.",
                "Database query optimization including index creation, query analysis, and performance tuning for improved response times.",
                "Comprehensive security audit logging capturing all user actions, system events, and security-related activities for compliance.",
                "Performance metrics collection system gathering system performance data, user interaction metrics, and resource utilization statistics.",
                "Automated testing suite including unit tests, integration tests, and end-to-end tests with automated test execution and reporting.",
                "Deployment pipeline automation with environment promotion, rollback capabilities, and deployment monitoring for reliable releases.",
                "Environment configuration management supporting multiple environments (dev, staging, prod) with environment-specific settings.",
                "Automated data backup system with scheduled backups, incremental backup support, and disaster recovery procedures.",
                "User activity tracking system monitoring user interactions, feature usage, and behavior patterns for product improvement.",
                "Advanced search and filtering capabilities with full-text search, faceted search, and saved search functionality.",
                "Bulk operations support for common tasks like user management, data import/export, and system maintenance operations.",
                "Email notification system with customizable templates, delivery tracking, and user preference management.",
                "System health monitoring with real-time status checks, alerting, and health dashboard for operational visibility.",
                "User preferences management system allowing users to customize their experience, save settings, and manage personal configurations."
            };
            return descriptions[index - 1];
        }

        private string GetTaskName(int index)
        {
            var names = new[] {
                "Update Login Form Validation",
                "Fix Mobile Responsiveness",
                "Add Unit Tests",
                "Update Documentation",
                "Optimize Database Query",
                "Add Error Handling",
                "Implement Caching",
                "Fix Navigation Bug",
                "Add Loading States",
                "Update Dependencies",
                "Fix Accessibility Issues",
                "Add Logging",
                "Optimize Images",
                "Fix Cross-browser Issues",
                "Add Progress Indicators",
                "Implement Retry Logic",
                "Add Input Validation",
                "Fix Memory Leak",
                "Add Error Boundaries",
                "Update API Endpoints",
                "Fix Performance Issue",
                "Add Data Validation",
                "Implement Pagination",
                "Fix Security Vulnerability",
                "Add Analytics Tracking",
                "Fix Search Bug",
                "Add Export Options",
                "Implement Sorting",
                "Fix Filter Issues",
                "Add Bulk Actions",
                "Fix Upload Bug",
                "Add Preview Functionality",
                "Implement Drag and Drop",
                "Fix Print Layout",
                "Add Keyboard Shortcuts",
                "Fix Timezone Issues",
                "Add Multi-language Support",
                "Fix Date Formatting",
                "Add Data Export",
                "Implement Search Highlighting"
            };
            return names[index - 1];
        }

        private string GetTaskDescription(int index)
        {
            var descriptions = new[] {
                "Update client-side validation for login form to prevent invalid submissions and improve user experience with better error messages.",
                "Fix mobile responsiveness issues in the main navigation and form components to ensure proper display on all device sizes.",
                "Add comprehensive unit tests for the authentication module to ensure code quality and reliability with good test coverage.",
                "Update API documentation to reflect recent changes and add examples for common use cases to improve developer experience.",
                "Optimize database queries in the reporting module to improve performance and reduce load times for better user experience.",
                "Add comprehensive error handling to prevent application crashes and provide user-friendly error messages for better UX.",
                "Implement Redis caching layer to improve performance and reduce database load for frequently accessed data.",
                "Fix navigation bug that causes incorrect routing when using browser back button and ensure proper navigation state.",
                "Add loading states to improve user experience during data fetching and long-running operations.",
                "Update project dependencies to latest stable versions to fix security vulnerabilities and improve performance.",
                "Fix accessibility issues to ensure compliance with WCAG guidelines and improve usability for users with disabilities.",
                "Add comprehensive logging to help with debugging and monitoring system health for better operational visibility.",
                "Optimize image loading by implementing lazy loading and proper image compression for improved page load performance.",
                "Fix cross-browser compatibility issues to ensure consistent behavior across different browsers and versions.",
                "Add progress indicators for long-running operations to improve user experience and provide operation feedback.",
                "Implement retry logic for failed API calls to improve reliability and handle temporary network issues gracefully.",
                "Add input validation to prevent invalid data entry and improve data quality with real-time validation feedback.",
                "Fix memory leak in dashboard component that causes increasing memory usage over time and eventual browser slowdown.",
                "Add error boundaries to prevent application crashes and provide graceful error handling for better user experience.",
                "Update API endpoints to use consistent naming conventions and improve API design for better developer experience.",
                "Fix performance issue that causes slow page loading and implement optimization techniques for better responsiveness.",
                "Add data validation to ensure data integrity and prevent invalid data from being stored in the system.",
                "Implement pagination for large data sets to improve performance and provide better user experience for data browsing.",
                "Fix security vulnerability that could allow unauthorized access and implement proper security measures.",
                "Add analytics tracking to monitor user behavior and gather insights for product improvement and optimization.",
                "Fix search bug that prevents users from finding relevant results and improve search algorithm accuracy.",
                "Add export options for data tables to allow users to download data in various formats for offline analysis.",
                "Implement sorting functionality for data tables to improve data organization and user experience.",
                "Fix filter issues that prevent proper data filtering and ensure filter functionality works correctly.",
                "Add bulk actions for common operations to improve efficiency and user experience for batch operations.",
                "Fix upload bug that prevents file uploads and ensure reliable file upload functionality.",
                "Add preview functionality for uploaded files to improve user experience and file management.",
                "Implement drag and drop functionality for better user interaction and improved user experience.",
                "Fix print layout issues to ensure proper printing and provide better print experience for users.",
                "Add keyboard shortcuts for common actions to improve accessibility and power user experience.",
                "Fix timezone issues that cause incorrect time display and ensure proper timezone handling.",
                "Add multi-language support to improve accessibility and user experience for international users.",
                "Fix date formatting issues to ensure consistent date display across different locales and formats.",
                "Add data export functionality to allow users to download their data in various formats.",
                "Implement search highlighting to improve search results visibility and user experience."
            };
            return descriptions[index - 1];
        }

        private string GetBugName(int index)
        {
            var names = new[] {
                "Login page crashes on mobile Safari",
                "Search results not displaying correctly",
                "File upload fails for large files",
                "Performance degradation under high load",
                "Memory leak in dashboard component",
                "API timeout errors during peak usage",
                "Database connection pool exhaustion",
                "Caching layer not working properly",
                "Email notifications not being sent",
                "User permissions not enforced correctly",
                "Data export fails for complex queries",
                "Real-time updates not working",
                "Session timeout too aggressive",
                "Error messages not user-friendly",
                "Mobile app crashes on startup",
                "Search pagination broken",
                "Filter options not working",
                "Sorting functionality broken",
                "Bulk operations failing",
                "Print functionality broken"
            };
            return names[index - 1];
        }

        private string GetBugDescription(int index)
        {
            var descriptions = new[] {
                "Users report that the login page crashes immediately when accessed from mobile Safari. This affects iOS 15+ devices and prevents users from logging in.",
                "Search functionality is returning results but they are not being displayed in the UI. The API is working correctly but the frontend rendering is broken.",
                "File uploads larger than 10MB are failing with a generic error message. Need to investigate the file size limits and error handling.",
                "Application performance significantly degrades when more than 100 concurrent users are active. Response times increase from 200ms to 2000ms+.",
                "Dashboard component is consuming increasing amounts of memory over time, eventually causing the browser to become unresponsive.",
                "API calls are timing out during peak usage hours (2-4 PM) when server load is highest. Need to investigate server capacity.",
                "Database connection pool is being exhausted during high traffic periods, causing connection errors and failed requests.",
                "Redis caching layer is not working as expected. Cache hits are very low and performance is not improved.",
                "Email notifications are not being sent to users despite the system showing them as sent. SMTP configuration may be incorrect.",
                "User permissions are not being properly enforced. Users can access features they shouldn't have access to.",
                "Data export functionality fails when users try to export complex datasets with many filters applied.",
                "Real-time updates using WebSockets are not working consistently. Some users receive updates while others don't.",
                "User sessions are timing out too quickly (after 15 minutes) which is causing frustration for users working on long tasks.",
                "Error messages displayed to users are too technical and not helpful. Need to make them more user-friendly.",
                "Mobile application crashes immediately upon startup on certain Android devices (API level 26-29).",
                "Search pagination is not working correctly, preventing users from accessing all search results.",
                "Filter options in data tables are not working, preventing users from filtering data effectively.",
                "Sorting functionality in data tables is broken, preventing users from organizing data properly.",
                "Bulk operations are failing when users try to perform actions on multiple items simultaneously.",
                "Print functionality is broken, preventing users from printing reports and data tables."
            };
            return descriptions[index - 1];
        }

        private string GetShowstopperName(int index)
        {
            var names = new[] {
                "Critical Security Vulnerability - SQL Injection",
                "System Down - Database Connection Failure",
                "Data Loss - Backup System Failure",
                "Authentication System Completely Broken",
                "Payment Processing System Down"
            };
            return names[index - 1];
        }

        private string GetShowstopperDescription(int index)
        {
            var descriptions = new[] {
                "Critical SQL injection vulnerability discovered in user input handling. Attackers can potentially access sensitive data and compromise the entire system. IMMEDIATE ACTION REQUIRED.",
                "Database connection system has completely failed. All users are unable to access the application and no data can be retrieved or stored. System is completely unusable.",
                "Backup system has failed and data recovery is not possible. Risk of permanent data loss if primary system fails. Need immediate investigation and recovery procedures.",
                "Authentication system is completely broken. No users can log in and all existing sessions have been invalidated. System is inaccessible to all users.",
                "Payment processing system is down. Users cannot complete purchases or transactions. This is directly impacting business revenue and customer experience."
            };
            return descriptions[index - 1];
        }

        private string GetCommentText(Item item, int commentIndex, Random random)
        {
            var templates = new[] {
                $"Initial review of {item.Name}. This looks like a solid approach and aligns with our architecture.",
                $"I've started working on this. The current implementation seems good but we might need to consider edge cases.",
                $"Updated the implementation based on feedback. Added better error handling and validation.",
                $"This is ready for testing. I've added comprehensive unit tests and updated the documentation.",
                $"Found a potential issue during testing. We should investigate the performance impact before proceeding.",
                $"Great work on this! The solution is elegant and well-tested. Ready for production deployment.",
                $"Need to discuss this with the team. There might be some architectural considerations we haven't addressed.",
                $"This is blocked by a dependency. Waiting for that to be completed first."
            };
            return templates[commentIndex - 1];
        }

        // Interface implementation methods
        public Task<string> ConnectedUserName() => Task.FromResult(_connectedUserName);

        public Task<Item?> GetBacklogItem(string id)
        {
            _items.TryGetValue(id, out var item);
            return Task.FromResult(item);
        }

        public Task<Item?> GetBug(string id)
        {
            var bug = _items.Values.FirstOrDefault(i => i.Id == id && i.Type == P4PlanLib.Model.ItemType.Bug);
            return Task.FromResult(bug);
        }

        public Task<List<Comment>> GetComments(string itemId)
        {
            if (_comments.TryGetValue(itemId, out var comments))
                return Task.FromResult(comments.ToList());

            return Task.FromResult(new List<Comment>());
        }

        public Task<List<Item>> GetItemChildrenAsync(string backlogEntryId, bool includeCompletedTasks = false)
        {
            if (_children.TryGetValue(backlogEntryId, out var children))
            {
                if (includeCompletedTasks)
                    return Task.FromResult(children.ToList());
                else
                    return Task.FromResult(children.Where(c => c.Status != "Done").ToList());
            }

            return Task.FromResult(new List<Item>());
        }

        public bool IsConnected() => true;

        public Task LoginAsync(string username, string password) => Task.CompletedTask;

        public Task<bool> PostComment(string itemId, string text)
        {
            if (!_comments.ContainsKey(itemId))
                _comments[itemId] = new List<Comment>();

            var newComment = new Comment
            {
                Id = $"{itemId}-C{_comments[itemId].Count + 1}",
                Text = text,
                CreatedAt = DateTime.Now,
                CreatedBy = _connectedUser
            };

            _comments[itemId].Add(newComment);
            return Task.FromResult(true);
        }

        public Task<T?> RunAsync<T>(GraphQLRequest request, string fieldPath)
        {
            if (typeof(T) == typeof(List<Item>))
            {
                var items = _items.Values.ToList();
                return Task.FromResult((T?)(object)items);
            }

            throw new NotImplementedException("Dummy client does not support raw GraphQL. Use the specific methods instead.");
        }

        // Search method
        public Task<List<Item>> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Task.FromResult(_items.Values.ToList());

            // Start with all items
            var results = _items.Values.ToList();
            // Step 1: Filter by Item Type
            results = FilterByItemType(results, query);
            // Step 2: Filter by Status
            results = FilterByStatus(results, query);
            // Step 3: Filter by Severity (for bugs)
            results = FilterBySeverity(results, query);
            // Step 4: Filter by Sprint/Commitment
            results = FilterBySprint(results, query);
            // Step 5: Filter by Assigned User
            results = FilterByAssignee(results, query);
            // Step 6: Filter by Release Tag (milestone)
            results = FilterByReleaseTag(results, query);
            // Step 7: Text search (if no specific filters applied)
            if (IsTextSearch(query))
            {
                results = FilterByText(results, query);
            }
            return Task.FromResult(results);
        }

        public Task<IEnumerable<string>> GetPrioritiesAsync()
        {
            var order = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            {
                ["veryHigh"] = 1,
                ["high"] = 2,
                ["medium"] = 3,
                ["low"] = 5,
                ["veryLow"] = 6,
            };

            var priorities = _items.Values
                .Select(i => i.Priority)
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .Select(p => p!)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(p => order.TryGetValue(p, out var rank) ? rank : 4)
                .ToList();

            return Task.FromResult<IEnumerable<string>>(priorities);
        }
        public Task<IEnumerable<string>> GetSprintsAsync()
        {
            var sprints = _items.Values
                .Select(i => i.CommittedTo?.Name)
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .Select(n => n!)
                .Distinct()
                .OrderBy(n => n)
                .ToList();
            return Task.FromResult<IEnumerable<string>>(sprints);
        }

        private List<Item> FilterByItemType(List<Item> items, string query)
        {
            if (query.Contains("Item type=bug") || query.Contains("Item type=\"bug\"") || query.Contains("\"Item type\"=bug"))
            {
                return items.Where(i => i.Type == P4PlanLib.Model.ItemType.Bug).ToList();
            }
            else if (query.Contains("Item type=\"backlog item\"") || query.Contains("Item type!=\"bug\"") || query.Contains("\"Item type\"=\"backlog item\""))
            {
                return items.Where(i => i.Type == P4PlanLib.Model.ItemType.Backlog).ToList();
            }

            return items; // No type filter applied
        }

        private List<Item> FilterByStatus(List<Item> items, string query)
        {
            if (query.Contains("Status!=Complete") || query.Contains("Status!=Done"))
            {
                return items.Where(i => i.Status != "Done").ToList();
            }

            return items; // No status filter applied
        }

        private List<Item> FilterBySeverity(List<Item> items, string query)
        {
            if (query.Contains("Severity >\"Severity B\""))
            {
                // High severity bugs (High, Critical)
                return items.Where(i => i.Severity == "High" || i.Severity == "Critical").ToList();
            }
            else if (query.Contains("Severity <=\"Severity B\""))
            {
                // Low/Medium severity bugs
                return items.Where(i => i.Severity == "Low" || i.Severity == "Medium").ToList();
            }

            return items; // No severity filter applied
        }

        private List<Item> FilterBySprint(List<Item> items, string query)
        {
            if (query.Contains("Committed to\":\"S30\"") || query.Contains("\"Committed to\":\"S30\""))
            {
                // Sprint items (not in backlog)
                return items.Where(i => i.CommittedTo?.Name != "Backlog").ToList();
            }

            return items; // No sprint filter applied
        }

        private List<Item> FilterByAssignee(List<Item> items, string query)
        {
            var assignedToMatch = System.Text.RegularExpressions.Regex.Match(query, @"""Assigned to"":""([^""]+)""|Assigned to"":""([^""]+)""");
            if (assignedToMatch.Success)
            {
                var userName = assignedToMatch.Groups[1].Value ?? assignedToMatch.Groups[2].Value;
                return items.Where(i => i.AssignedTo?.Any(a => a.User?.Name == userName) == true).ToList();
            }

            var assignTagMatch = System.Text.RegularExpressions.Regex.Match(query, @"""Assign tag"":""([^""]+)""|Assign tag"":""([^""]+)""");
            if (assignTagMatch.Success)
            {
                var userName = assignTagMatch.Groups[1].Value ?? assignTagMatch.Groups[2].Value;
                return items.Where(i => i.AssignedTo?.Any(a => a.User?.Name == userName) == true).ToList();
            }

            return items; // No assignee filter applied
        }

        private List<Item> FilterByReleaseTag(List<Item> items, string query)
        {
            if (query.Contains("Release tag"))
            {
                // Milestone items - return items that are not done
                return items;
            }

            return items; // No release tag filter applied
        }

        private bool IsTextSearch(string query)
        {
            // Check if this is a simple text search (no specific filters)
            var hasFilters = query.Contains("Item type") ||
                           query.Contains("Status") ||
                           query.Contains("Severity") ||
                           query.Contains("Committed to") ||
                           query.Contains("Assigned to") ||
                           query.Contains("Assign tag") ||
                           query.Contains("Release tag");

            return !hasFilters;
        }

        private List<Item> FilterByText(List<Item> items, string query)
        {
            return items.Where(item =>
                item.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                item.Description.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                item.Id.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public Task<IEnumerable<string>> GetAssigneesAsync(string? search)
        {
            var names = _items.Values
                .SelectMany(i => i.AssignedTo ?? Array.Empty<AssignedTo>())
                .Select(a => a.User?.Name)
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .Select(n => n!)
                .Distinct(StringComparer.OrdinalIgnoreCase);
            if (!string.IsNullOrWhiteSpace(search))
            {
                names = names.Where(n => n.Contains(search, StringComparison.OrdinalIgnoreCase));
            }
            return Task.FromResult<IEnumerable<string>>(names.OrderBy(n => n).Take(20).ToList());
        }

    }
}
