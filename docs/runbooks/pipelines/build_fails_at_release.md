# Create a GitHub Release

## Symptoms
When running a master release the build pipeline fails, due to the release already existing.

## Solution
1. Open the workflow logs at the error and expand the logs, so you can see all the inputs.
2. Record the release number
3. Have an admin: 
  - Delete the release from the releases
  - Delete the tag
