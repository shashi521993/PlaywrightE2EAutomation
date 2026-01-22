E2E Shopping Automation Framework - Playwright .NET


Overview:
	This repository contains a professional automation framework designed to validate a complete shopping journey on an e-commerce platform (DemoWebShop). The framework is built using Playwright .NET (2026), C#, and NUnit, demonstrating advanced techniques for handling dynamic web elements and data-driven scenarios.


Architecture:
	The framework is designed using the Page Object Model (POM) pattern to ensure a clean separation between UI interactions and test logic.

	BasePage: A centralized class providing shared utilities for synchronized clicks, screenshot capturing, and custom logging.

	Data-Driven Design: Test configurations (BaseURL, Search Query, Price Limits) and user credentials are managed via external JSON files in the /Config directory.

	Robustness: Elements are located using Playwright's modern locators (AriaRole, GetByText) combined with auto-waiting mechanisms to handle asynchronous UI updates.


The End-to-End Scenario (Core Focus):
	The primary deliverable of this framework is the Full E2E Scenario, which simulates a realistic user journey:

	Authentication: Secure login using externalized credentials.

	Smart Cart Cleanup: Immediately after login, the framework detects if the shopping cart contains items from previous runs. If so, it automatically clears the cart to ensure a consistent starting state (Clean State).

	Product Search & Filtering: Performs a search and filters results dynamically based on price limits and paging availability (Requirement 4.1).

	Generic Item Handling: Handles complex product pages by automatically selecting random variants (such as Size, RAM, or HDD) before adding items to the cart (Requirement 4.2).

	Validation: Final verification of the total cart amount against a defined budget threshold (Requirement 4.3).


Project Structure:

	Pages: Page Object classes representing the UI components.

	Tests:
		EndToEndTests.cs: The main E2E test suite (Primary Focus).
		ComponentTests: Supporting tests for individual function validation (Login, Search, Cart).

	Config: JSON files for environment and data management.


Execution and Reporting:

	Prerequisites
		.NET 8.0 SDK
		Playwright Browsers (To install, run: pwsh bin/Debug/net8.0/playwright.ps1 install)

	Running the Tests
		To run the Main E2E Scenario with a visible browser (Headed mode):
		dotnet test --filter "Name=FullShoppingFlow_E2E" --settings playwright.runsettings
		Note: The playwright.runsettings file is configured to launch the browser in headed mode with a slight delay (SlowMo) for visual confirmation.

	Reporting and Artifacts
		HTML Trace Reports (Requirement 6): For every E2E run, a comprehensive Playwright Trace (HTML-based report) is generated.
			Location: /TestReports/trace_*.zip
			How to View: Drag the generated ZIP file into trace.playwright.dev. This provides a step-by-step visual replay, including network logs and source code.

		Screenshots: Automatically captured during the "Add to Cart" and budget validation steps, stored in the /Screenshots folder at the project root.


Assumptions & Limitations:

	Platform: DemoWebShop was utilized as the target platform to demonstrate complex attribute handling and paging.

	Currency: Logic assumes decimal-based pricing (USD) as per the site's default.

	State Isolation: The automatic cart-reset mechanism ensures test reliability across consecutive runs.

	The framework uses a fallback URL mechanism in case configuration files are missing, ensuring stability across different environments.