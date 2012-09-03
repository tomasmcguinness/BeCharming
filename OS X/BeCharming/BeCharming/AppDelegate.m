//
//  AppDelegate.m
//  BeCharming
//
//  Created by Tomas McGuinness on 02/09/2012.
//  Copyright (c) 2012 tomasmcguinness.com. All rights reserved.
//

#import "AppDelegate.h"

@implementation AppDelegate

- (void)dealloc
{
    [super dealloc];
}

- (void)applicationDidFinishLaunching:(NSNotification *)aNotification
{
    self.listener = [[Listener alloc] init];
    [self.listener start];
    
    self.statusItem = [[NSStatusBar systemStatusBar] statusItemWithLength:40.0];
    self.statusItem.title = @"BeCharming";
}

- (void)applicationWillTerminate:(NSNotification *)notification
{
    [self.listener stop];
    self.listener = nil;
}

@end
