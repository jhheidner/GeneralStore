# General Store Mobile Tests

This repository contains automated tests for the General Store Android application using Appium and .NET.

## Test Cases

The test suite includes the following test cases:

### 1. Form Submission Tests

#### `ShouldSubmitFormSuccessfully`

Verifies that users can successfully submit the initial form with valid data:

- Selects country (Brazil)
- Enters name (John Doe)
- Selects gender (male)
- Submits form
- Verifies successful navigation to products page

#### `ShouldShowErrorWhenNameIsMissing`

Validates form validation when required fields are missing:

- Selects country (Brazil)
- Selects gender (female)
- Attempts to submit without name
- Verifies error toast message "Please enter your name"

### 2. Shopping Cart Tests

#### `ShouldAddMultipleItemsToCart`

Tests the ability to add multiple items to the shopping cart:

- Adds three different products:
  - Jordan 6 Rings
  - Nike SFB Jungle
  - Converse All Star
- Verifies all three items appear in cart
- Handles scrolling to find all items in cart view

#### `ShouldCalculateCartTotalCorrectly`

Validates the cart total calculation functionality:

- Adds multiple items to cart
- Retrieves individual product prices
- Calculates expected total
- Verifies cart total matches sum of items

## Azure DevOps Pipeline Setup

### Prerequisites

1. Azure DevOps account and project
2. Repository connected to Azure DevOps
3. Permissions to create and manage pipelines

### Setting Up the Pipeline

1. In Azure DevOps, navigate to Pipelines
2. Click "New Pipeline"
3. Select your repository source
4. Choose "Existing Azure Pipelines YAML file"
5. Select the `azure-pipelines.yml` file
6. Click "Continue" and then "Run"

### Pipeline Features

The pipeline includes:

- ✅ Automatic build on main branch changes
- ✅ Windows build agent setup
- ✅ Android SDK and Emulator configuration
- ✅ Appium server setup
- ✅ Test execution with reporting
- ✅ Code coverage collection
- ✅ Test results publication

### Environment Configuration

The pipeline automatically sets up:

- Node.js 18.x
- Appium 2.18.0
- Android SDK with API 33
- Pixel 5 Emulator
- Required Android tools and drivers

### Test Results

After pipeline execution, you can find:

1. Test results in the Tests tab
2. Code coverage report in the Code Coverage tab
3. Detailed test execution logs in the pipeline run

### Troubleshooting

Common issues and solutions:

1. **Emulator fails to start**

   - Check Android SDK installation
   - Verify emulator configuration
   - Check system resources

2. **Appium connection issues**

   - Verify Appium server status
   - Check port availability
   - Confirm UiAutomator2 driver installation

3. **Test execution failures**
   - Check test logs for details
   - Verify app installation
   - Confirm emulator is fully booted

### Local Development

To run tests locally:

1. Install prerequisites:

   ```powershell
   npm install -g appium@2.18.0
   appium driver install uiautomator2
   ```

2. Start Appium server:

   ```powershell
   appium
   ```

3. Run tests:
   ```powershell
   dotnet test
   ```

## Contact

For issues or questions, @jhheidner@gmail.com.
