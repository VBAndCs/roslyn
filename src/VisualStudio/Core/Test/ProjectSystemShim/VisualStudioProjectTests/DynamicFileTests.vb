﻿' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.

Imports System.Collections.Immutable
Imports System.Threading
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Test.Utilities
Imports Microsoft.CodeAnalysis.UnitTests
Imports Microsoft.VisualStudio.LanguageServices.UnitTests.ProjectSystemShim.Framework
Imports Roslyn.Test.Utilities

Namespace Microsoft.VisualStudio.LanguageServices.UnitTests.ProjectSystemShim
    <[UseExportProvider]>
    Public Class DynamicFileTests
        <WpfFact>
        Public Sub AddAndRemoveFileWhenDynamicFileInfoProviderProducesNothing()
            Using environment = New TestEnvironment(GetType(TestDynamicFileInfoProviderThatProducesNoFiles))
                Dim project = environment.ProjectFactory.CreateAndAddToWorkspace(
                    "project",
                    LanguageNames.CSharp)

                Const DynamicFileName As String = "DynamicFile.cshtml"

                project.AddDynamicSourceFile(DynamicFileName, ImmutableArray(Of String).Empty)

                project.RemoveDynamicSourceFile(DynamicFileName)
            End Using
        End Sub

        <WpfFact>
        Public Sub AddAndRemoveFileWhenDynamicFileInfoProviderProducesSomething()
            Using environment = New TestEnvironment(GetType(TestDynamicFileInfoProviderThatProducesFiles))
                Dim project = environment.ProjectFactory.CreateAndAddToWorkspace(
                    "project",
                    LanguageNames.CSharp)

                Const DynamicFileName As String = "DynamicFile.cshtml"

                project.AddDynamicSourceFile(DynamicFileName, ImmutableArray(Of String).Empty)

                Dim dynamicSourceFile = environment.Workspace.CurrentSolution.Projects.Single().Documents.Single()

                Assert.Equal(
                    TestDynamicFileInfoProviderThatProducesFiles.GetDynamicFileText(DynamicFileName),
                    dynamicSourceFile.GetTextSynchronously(CancellationToken.None).ToString())

                project.RemoveDynamicSourceFile(DynamicFileName)

                Assert.Empty(environment.Workspace.CurrentSolution.Projects.Single().Documents)
            End Using
        End Sub

        <WpfFact>
        Public Sub AddAndRemoveFileAndAddAgain()
            Using environment = New TestEnvironment(GetType(TestDynamicFileInfoProviderThatProducesFiles))
                Dim project = environment.ProjectFactory.CreateAndAddToWorkspace(
                    "project",
                    LanguageNames.CSharp)

                Const DynamicFileName As String = "DynamicFile.cshtml"

                project.AddDynamicSourceFile(DynamicFileName, ImmutableArray(Of String).Empty)

                project.RemoveDynamicSourceFile(DynamicFileName)

                project.AddDynamicSourceFile(DynamicFileName, ImmutableArray(Of String).Empty)
            End Using
        End Sub

        <WpfFact>
        Public Sub AddAndRemoveExtensionlessFile()
            Using environment = New TestEnvironment(GetType(TestDynamicFileInfoProviderThatProducesFiles))
                Dim project = environment.ProjectFactory.CreateAndAddToWorkspace(
                    "project",
                    LanguageNames.CSharp)

                Const DynamicFileName As String = "DynamicFile"

                project.AddDynamicSourceFile(DynamicFileName, ImmutableArray(Of String).Empty)

                Assert.Empty(environment.Workspace.CurrentSolution.Projects.Single().Documents)

                project.RemoveDynamicSourceFile(DynamicFileName)

                Assert.Empty(environment.Workspace.CurrentSolution.Projects.Single().Documents)
            End Using
        End Sub
    End Class
End Namespace

