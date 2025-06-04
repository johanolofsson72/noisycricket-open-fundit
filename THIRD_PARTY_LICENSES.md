# Third-Party Licenses and Dependencies

This project uses several third-party libraries and components, some of which require commercial licenses for certain use cases. Please review the license requirements for each component before using this project.

## Commercial Components (Requires License)

### Telerik UI for Blazor
- **Package**: `Telerik.UI.for.Blazor`
- **License**: Commercial license required for commercial use
- **Free Option**: Available for non-commercial and open source projects
- **Website**: https://www.telerik.com/blazor-ui
- **Note**: You need to configure your Telerik credentials in `src/Common/NuGet.config`

### Syncfusion Blazor Components
- **Package**: `Syncfusion.Blazor.*`
- **License**: Commercial license required for commercial use
- **Free Option**: Community license available for qualifying users
- **Website**: https://www.syncfusion.com/blazor-components
- **Note**: Free for individual developers and small businesses with revenue < $1M

### Telerik Document Processing Libraries
- **Packages**: `Telerik.Documents.Core`, `Telerik.Documents.Flow`, `Telerik.Zip`
- **License**: Commercial license required for commercial use
- **Website**: https://www.telerik.com/document-processing-libraries

## Text Processing Components (Custom/Unknown License)
The following DLL files are included in the project but their license status is unclear:
- `OpenNLP.dll`
- `PageRank.dll` 
- `SharpEntropy.dll`
- `TextRank.dll`
- `TextSummarizer.dll`

**Important**: Please verify the license terms for these components before commercial use.

## Open Source Components
All other dependencies use permissive open source licenses (MIT, Apache 2.0, BSD, etc.) and are freely available for commercial use.

## Usage Guidelines

### For Open Source Projects
- Telerik and Syncfusion offer free licenses for open source projects
- Apply for free licenses at their respective websites
- Configure credentials in `src/Common/NuGet.config`

### For Commercial Projects
- Purchase appropriate commercial licenses for Telerik and Syncfusion components
- Review license terms for the custom DLL files
- Ensure compliance with all third-party license requirements

### For Evaluation/Development
- Most components offer free trial periods
- Some have community editions with limitations
- Check individual component websites for trial options

## Getting Licensed Components

1. **Telerik UI for Blazor**: Visit https://www.telerik.com/blazor-ui
2. **Syncfusion Components**: Visit https://www.syncfusion.com/blazor-components
3. **Text Processing DLLs**: Contact the project maintainer for license information

## Removing Commercial Dependencies

If you prefer to avoid commercial licenses, you can:
1. Remove Telerik components and replace with open source alternatives
2. Remove Syncfusion components and use built-in Blazor components
3. Replace text processing DLLs with open source NLP libraries

See the documentation for guidance on using alternative components.

---

**Disclaimer**: This information is provided for guidance only. Always verify license terms directly with component vendors and consult legal counsel for commercial use cases.
