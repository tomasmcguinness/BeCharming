//
//  AppDelegate.h
//  BeCharming
//
//  Created by Tomas McGuinness on 02/09/2012.
//  Copyright (c) 2012 tomasmcguinness.com. All rights reserved.
//

#import <Cocoa/Cocoa.h>
#import "Listener.h"

@interface AppDelegate : NSObject <NSApplicationDelegate>

@property (assign) IBOutlet NSWindow *window;
@property (retain) Listener *listener;

@property (nonatomic, retain) NSStatusItem *statusItem;

@end
