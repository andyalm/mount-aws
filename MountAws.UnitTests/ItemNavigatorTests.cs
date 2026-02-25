using System;
using System.Collections.Generic;
using System.Linq;
using AwesomeAssertions;
using MountAnything;
using Xunit;

namespace MountAws.UnitTests;

public class ItemNavigatorTests
{
    private readonly FileNavigator _navigator;

    public ItemNavigatorTests()
    {
        var paths = new[]
        {
            "gitlab-container/Gitlab-Web-shared/12345",
            "gitlab-container/Gitlab-Web-shared/23456",
            "gitlab-container/Gitlab-Web-shared/34567",
            "gitlab-container/Gitlab-Web-shared/34567",
            "2018/03/07/[$LATEST]0b6252608cdb4f57a51513efac0edd42",
        };
        _navigator = new FileNavigator(paths);
    }
    
    [Fact]
    public void ListsChildDirectoriesDirectories()
    {
        var childItems = _navigator.ListChildItems(new ItemPath("cloudwatch/log-groups/myloggroup/streams")).ToArray();

        childItems.Should().HaveCount(1);
        childItems[0].ItemName.Should().Be("gitlab-container");
        childItems[0].ParentPath.FullName.Should().Be("cloudwatch/log-groups/myloggroup/streams");
    }
    
    [Fact]
    public void ListsChildDirectoriesDirectories2()
    {
        var childItems = _navigator.ListChildItems(new ItemPath("cloudwatch/log-groups/myloggroup/streams/gitlab-container"), new ItemPath("gitlab-container")).ToArray();

        childItems.Should().HaveCount(1);
        childItems[0].ItemName.Should().Be("Gitlab-Web-shared");
        childItems[0].ParentPath.FullName.Should().Be("cloudwatch/log-groups/myloggroup/streams/gitlab-container");
    }
    
    [Fact]
    public void ListsChildDirectoriesDirectories3()
    {
        var childItems = _navigator.ListChildItems(new ItemPath("cloudwatch/log-groups/aws/lambda/mynotify/streams/2018"), new ItemPath("2018")).ToArray();

        childItems.Should().HaveCount(1);
        childItems[0].ItemName.Should().Be("03");
        childItems[0].ParentPath.FullName.Should().Be("cloudwatch/log-groups/aws/lambda/mynotify/streams/2018");
    }
    
    private class File
    {
        public File(ItemPath path)
        {
            Path = path;
        }

        public ItemPath Path { get; }
    }

    private class FileItem : Item<File>
    {
        public FileItem(ItemPath parentPath, File file, string itemType) : base(parentPath, file)
        {
            ItemName = file.Path.Name;
            ItemType = itemType;
        }

        public override string? ItemType { get; }

        public override string ItemName { get; }
        public override bool IsContainer => false;
    }

    private class FileNavigator : ItemNavigator<File, FileItem>
    {
        private readonly IEnumerable<File> _files;
        
        public FileNavigator(IEnumerable<string> filePaths)
        {
            _files = filePaths.Select(p => new File(new ItemPath(p)));
        }

        public FileNavigator(IEnumerable<ItemPath> filePaths)
        {
            _files = filePaths.Select(p => new File(p));
        }

        protected override FileItem CreateDirectoryItem(ItemPath parentPath, ItemPath directoryPath)
        {
            return new FileItem(parentPath, new File(directoryPath), "Directory");
        }

        protected override FileItem CreateItem(ItemPath parentPath, File model)
        {
            return new FileItem(parentPath, model, "File");
        }

        protected override ItemPath GetPath(File model)
        {
            return model.Path;
        }

        protected override IEnumerable<File> ListItems(ItemPath? pathPrefix)
        {
            return _files;
        }
    }
}