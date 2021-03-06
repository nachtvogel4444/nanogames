#!/usr/bin/env perl

# Copyright (c) the authors of nanoGames. All rights reserved.
# Licensed under the MIT license. See LICENSE.txt in the project root.

use strict;
use warnings;
use File::Find;

my $cs_exclude = q{<auto-generated>};
my $cs_cut = q{(?:[ \t\r]+\n|[ \r]*//[^\r\n]*\r?\n)*};
my $cs_comment = "// Copyright (c) the authors of nanoGames. All rights reserved.\r\n// Licensed under the MIT license. See LICENSE.txt in the project root.\r\n\r\n";

find(\&process_file, '.');

sub process_file {
	if (/(?<!\.g)\.(?:cs|glsl)$/) { add_comments($cs_exclude, $cs_cut, $cs_comment); }
}

sub add_comments {
	my ($exclude, $cut, $comment) = @_;

	local $/;
	my $path = $File::Find::name;

	open(my $file, '<', $_) or die "Error reading '$path': $!";
	my $content = <$file>;
	close $file;

	# skip empty files
	if ($content =~ /^\s*$/s) { return; }

	# skip files with the exclude marker
	if ($content =~ /$exclude/) { return; }

	my $new_content = $content;
	$new_content =~ s/^((?:\xEF\xBB\xBF)?)$cut/$1$comment/s;

	if ($new_content ne $content) {
		print "$path\n";
		
		open(my $file, '>', $_) or die "Error writing '$path': $!";
		print $file $new_content;
		close $file;
	}
}
